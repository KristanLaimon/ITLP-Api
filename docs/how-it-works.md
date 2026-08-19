# How TecWrapperAPI talks to siia2

siia2 (`https://siia2.lapaz.tecnm.mx`) has no public API. It's a server-rendered
Laravel app: every page is plain HTML, login is a normal form POST, and "data" is
just text sitting inside `<p>`/`<td>` tags. This wrapper works by pretending to be
a browser, POSTing the login form like a browser would, keeping the session
cookie, then downloading the HTML pages a logged-in browser would see and
picking the numbers back out of the markup. No hidden API, no JSON endpoint.

## 1. The three requests

`TecApi.cs` talks to exactly three URLs:

| Step | URL | What it gets |
|---|---|---|
| `LoginAsync()` | `GET /login` then `POST /login` | the login page (for its CSRF token), then a logged-in session |
| `GetStudentStatusAsync()` | `GET /alumnos` | the kardex summary page (name, career, GPA...) |
| `GetGradeHistoryAsync()` | `GET /alumnos/historial-academico` | the full semester-by-semester grade table |

All three go through `HttpClient`, configured with a `CookieContainer`
(`TecApi.cs:38`) — that's the entire "session" mechanism. Laravel sets a
session cookie on the first response; `HttpClientHandler` stores it and resends
it automatically on every later request, exactly like a browser does. There's
no token to manage by hand once you're past login.

## 2. Why login needs a GET before the POST

Laravel protects forms with a CSRF token: a random string embedded in the login
page as `<input type="hidden" name="_token" value="...">`, tied to your session.
Submit the form without that exact token and the request is rejected as a
different session. So `LoginAsync()`:

1. `GET /login` — fetch the page, which also sets the first session cookie.
2. Pull the token out of the HTML (`ExtractCsrfTokenOrThrow`, `TecApi.cs:131`)
   using `HtmlPageHelper.FindFirst<IHtmlInputElement>(html, "input[name=\"_token\"]")`
   — a CSS selector query, not string-splitting.
3. `POST /login` with `numero`, `password`, and that `_token`
   (`HeadersHelper.GetAuthFormValues`), as a normal
   `application/x-www-form-urlencoded` body — same as the HTML `<form>` itself.

## 3. How "did login work?" is decided

siia2 doesn't reply with a 401 on bad credentials — it just re-renders the
*login page* again, this time with an error banner, and still returns HTTP 200.
So success/failure is detected by *what page came back*, not by status code:

`HtmlPageHelper.IsLoginPage()` checks whether the response HTML still contains
`<input name="numero">`. If it does, you're still looking at the login form,
so login failed. `ThrowIfLoginFailed()` (`TecApi.cs:144`) then regexes out the
error message the page embedded in `data-errors='["..."]'` on the error modal,
and throws `TecApiInvalidCredentialsException` with that exact message.
If the login page is gone, you're in.

## 4. Parsing: CSS selectors, not regex, over the HTML

Everything downstream reuses one idea from `HtmlPageHelper`: parse the HTML
into a real DOM with **AngleSharp** (`new HtmlParser().ParseDocument(html)`),
then query it with CSS selectors like a browser's `document.querySelector`
would. This survives whitespace/formatting noise in the page that a naive
regex-over-raw-HTML would choke on.

- **`StudentStatusParser.cs`** — the kardex page renders each field as
  `<p><strong>Nombre: </strong>JUAN PEREZ GOMEZ</p>`. The parser selects every
  `div.siia--inicio p`, splits each one on the first `": "`, and builds a
  `{"Nombre": "JUAN PEREZ GOMEZ", "Control": "...", ...}` dictionary — so it
  doesn't care what order the fields appear in, only that the label text is
  stable. A couple of fields get extra regex, e.g. `"240 (92.3%)"` for credits
  is split into count + percentage with `CreditosRegex`.

- **`GradeHistoryParser.cs`** — the grade history page is one big
  `<table class="tabla">` where each semester is a `<thead>` (with a
  `Semestre: N Periodo: ...` header row) followed by a `<tbody>` of subject
  rows. The parser walks the table's direct children in document order,
  remembering the last `<thead>` it saw as "current semester", and attaches
  every following `<tbody>` row to it — until a `Promedio del semestre` row
  closes that semester out. This is why it's a hand-written walk instead of a
  single CSS-selector query: the table has no `id`/`class` tying a `<tbody>`
  to its `<thead>`, so the *order* of the elements is the only thing that
  encodes "which semester is this row in."

## 5. What happens when Tec changes the page

Every parser throws `TecApiParsingException` (not a null or a crash) the moment
an expected selector/field goes missing — e.g. "Couldn't find the grades table
on the page. (Tec changed the UI layout?)". That's deliberate: since this is
screen-scraping, not a real API, the contract is *the site's current markup*,
which can change without notice. A typed exception with that context makes it
obvious the fix is "go look at the new HTML", not "there's a bug in the parser".

## 6. Looking believable to the server

`HeadersHelper.CommonHeadersDict` attaches a realistic Firefox `User-Agent` and
the browser-style `Sec-Fetch-*`/`Accept-*` headers real navigation requests
carry, and `Referer: /login`. None of this is required for the HTML to parse —
it just keeps requests from looking obviously scripted to whatever's serving
the page.

## 7. End-to-end

```
LoginAsync()
  GET  /login              -> HTML has <input name="_token" value="X">
  POST /login  {numero, password, _token: X}   (cookie jar now holds session)
  response HTML still has <input name="numero">?
      yes -> extract data-errors, throw TecApiInvalidCredentialsException
      no  -> IsLoggedIn = true

GetStudentStatusAsync()
  GET /alumnos              (cookie sent automatically)
  parse <p> tags in div.siia--inicio -> StudentStatus

GetGradeHistoryAsync()
  GET /alumnos/historial-academico
  parse table.tabla thead/tbody sequence -> List<SemesterHistory>
```

That's the whole trick: HTML parsing + a cookie jar, standing in for an API
that doesn't exist.
