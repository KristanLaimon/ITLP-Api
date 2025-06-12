# ITLP Kardex Api Wrapper

<div align="center">
    <img src="./.github/resources/csharp.png" alt="itlp_logo_png" width="20%">
    <img src="./.github/resources/itlp_logo.png" alt="itlp_logo_png" width="30%">
</div>

A direct and easy kardex access from C# library designed to interface
with the "Instituto Tecnológico de La Paz" school's Kardex system
"An academic records platform where students can view their grades"

The wrapper simplifies communication with the underlying ASP.NET-based API/page,
allowing for asynchronous connections and secure credential management.

## Key Features:
- ⚡ Asynchronous API Requests — Clean and simple use of async/await for all network operations.
- 🔐 Credential Management — User credentials stored and managed securely.
- 🎓 Access to Kardex Data — Easily fetch student grades, subjects, and academic progress.
- 🧩 Easy Integration — Ready to plug into any .NET 9.0+ C# project.
- 📝 Clean API Design — Simple, developer-friendly method calls.

## Why this project?
The official Kardex page for the Instituto Tecnológico de La Paz provides no public API or at least an
easy way to access the data from students (grades, academic situation, etc) programatically.
This wrapper automates and abstracts that interaction, making it easier for any students/(c# developers)
to integrate Kardex functionality into their C# applications (e.g., personal dashboards, bots, or tools)
if they want to, of course.

## Security
The password and username are used only the moment you instanciate the api object and permanently deleted
when closing the C# program. It's NOT STORED in any PLACE.
If you don't trust, you can simply check the code here in this repo.

## Disclaimer
This is not an official wrapper api, was made for educational and learning purposes.

🏗️🚧 This is under development, so it can be unstable at the time writing this readme 🏗️🚧

When finished, will be released as .ddl library here on github and as a NUGET package.

If you find this useful let me know giving a star to this repo!

## LICENSE
MIT. You can do as you please with this code, BUT YOU MUST INCLUDING MIT licence as well in your proyect
WITHOUT change the actual MIT license.

💖 I would really appreciate if you give credits: @KristanLaimon 2025 💖