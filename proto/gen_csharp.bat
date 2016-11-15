protoc.exe --plugin=protoc-gen-sharpnet=protoc-gen-sharpnet.exe --sharpnet_out . --proto_path "." %*
@IF %ERRORLEVEL% NEQ 0 pause