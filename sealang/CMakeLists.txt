add_definitions(-D_SEALANG_LIB_)

set(SOURCE_FILES
    sealang.cpp

	sealang.h
)

add_library(sealang SHARED
	${SOURCE_FILES}
)

set(LIBS
  clangAST
  clangBasic
  clangFrontend
  clangIndex
  clangLex
  clangSema
  clangTooling
)

target_link_libraries(sealang
	${LIBS}
)
