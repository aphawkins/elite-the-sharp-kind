## Architecture

* Old games from 8 and 16 bit computers ported over to dotnet and C#.
* Games should retain their original look and feel, while also taking advantage of modern hardware and software capabilities.
* Prefer dotnet framework intrinsics and libraries, over third party libraries.
* As much code as possible should be in the 'Useful' libraries and only game specific code should be in the game projects themselves.
* The highest possible coding standards should be used.
* The code focus should be on correctness, security, maintainability, readability, and performance.
* Code should adhere to DRY (Don't Repeat Yourself), YAGNI (You Aren't Gonna Need It), KISS (Keep It Simple, Stupid), and SOLID principles.
* Technical debt should be prioritised over new features.
* Development prioritisation technique should use the MoSCoW method
* Embrace continuous refactoring, continuous automated unit testing, and continuous integration.
* OOP principles should be used where appropriate, but not at the expense of performance or simplicity.
* The game engine should be designed to be as modular and extensible as possible, allowing for easy addition of new features and functionality.
* The game engine may differ from established engines in that it may be architected more like a business application, allowing developers from a business application background to more easily understand and contribute to the codebase.
* Unit testing, integration testing, and end-to-end testing should be used to ensure the quality and reliability of the codebase.
* Performance profiling and optimization should be an ongoing process throughout development, with a focus on identifying and addressing performance bottlenecks early in the development cycle.
* Code should be cross-platform, with support for Windows, Linux, x64, and ARM64 architectures. 32 bit architectures are not supported.

