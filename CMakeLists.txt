cmake_minimum_required (VERSION 2.8)
project (sealang)

# CMake Customization
set (LIBRARY_OUTPUT_PATH ${CMAKE_SOURCE_DIR}/output)
set (EXECUTABLE_OUTPUT_PATH ${CMAKE_SOURCE_DIR}/output)

# Dependencies

# llvm
set(LIBCLANG_BUILD_STATIC ON CACHE BOOL
  "Build libclang as a static library (in addition to a shared one)")
set(CLANG_BUILD_TOOLS OFF CACHE BOOL
	"Build the Clang tools. If OFF, just generate build targets.")
set(CLANG_INCLUDE_DOCS OFF CACHE BOOL
	"Generate build targets for the Clang docs.")
set(LLVM_BUILD_UTILS OFF CACHE BOOL
	"Build LLVM utility binaries. If OFF, just generate build targets.")
set(LLVM_INCLUDE_DOCS OFF CACHE BOOL
	"Generate build targets for llvm documentation.")
set(LLVM_INCLUDE_EXAMPLES OFF CACHE BOOL
	"Generate build targets for the LLVM examples")
set(LLVM_INCLUDE_GO_TESTS OFF CACHE BOOL
	"Include the Go bindings tests in test build targets.")
set(LLVM_INCLUDE_TESTS OFF CACHE BOOL
	"Generate build targets for the LLVM unit tests.")
set(LLVM_INCLUDE_UTILS OFF CACHE BOOL
	"Generate build targets for the LLVM utils.")

add_subdirectory ("llvm")

# sealang
include_directories ("llvm/include")
include_directories ("llvm/tools/clang/include")
add_subdirectory ("sealang")
