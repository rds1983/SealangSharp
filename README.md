# Overview

SealangSharp is C# port of that: [**sealang**](https://github.com/pybee/sealang)

Or - in other words - it is enhanced version of this: [**ClangSharp**] (https://github.com/Microsoft/ClangSharp)

It provides additional methods (determining exact type of operators and literal values) for analyzing the C source code.

See this file for the library interface: [sealang.cs](https://github.com/rds1983/SealangSharp/blob/master/SealangSharp/sealang.cs)

## Usage

The easiest way to use SealangSharp is to add it through the Nuget: install-package SealangSharp.
However it works only under Windows x64. SealangSharp includes the ClangSharp already, therefore it shouldn't be in the project to avoid conflicts.

This file demonstrates example usage of SealangSharp: [Program.cs] (https://github.com/rds1983/SealangSharp/blob/master/SealangSharpTest/Program.cs)

## Building from the Source Code
### Prerequisites
* CMake 3.4.3+
* VS2013+

### Building Native Libraries
1. Clone SealangSharp
2. Run VS x64 Native Tools Command Prompt
3. Cd build
4. Execute run_cmake.bat
5. Execute build_native.bat
6. Native libraries (libclang.dll and sealang.dll) should appear at correspondingly "llvm/Release/bin" and "sealang/Release" 

### Building Managed Libraries

