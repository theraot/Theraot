Theraot's Libraries
===

Theraot's Libraries are an ongoing effort to ease the work on .NET, including a backport of recent .NET features to .NET 2.0 among other things.

---
Introduction
---

Theraot.Core is as close as I am to a ".NET Compatibility Pack" capable to bring code from recent .NET versions back to .NET 2.0.

Please refer to the Wiki for more information.

---
Build
---

The compiling configuration has been set for ease of batch building from Visual Studio, building from Xamarin Studio is also supported.

The solution can also be built from `build.bat`, which is also used to create the NuGet Package.

---
Tests
---

This repository includes a test proyect for NUnit 2.6.3 you can use any NUnit test runner to execute the tests. The library is included nunit.framework.dll for ease of use.

To run the unit test you can use one of the following:
  - NUnit test runner included in the NUnit installation available from nunit.org
  - From Xamarin Studio using the NUnit integration
  - From Visual Studio using the Test Explorer with NUnit Test Adapter available from visualstudiogallery.msdn.microsoft.com
  - From Visual Studio with TestDriven.net
  - From Visual Studio with Reshaper
  - Any other test runner compatible with NUnit

---
License
---

This project is under MIT license, refer to LICENSE.txt for the license text.

The reason for this license is that this library includes code from Mono under MIT License.

This repository also includes a copy of NUnit 2.6.3 which includes its own license file.

---
Warranty
---

Aside from the license, I can only warranty the following: It did work on my machine.
