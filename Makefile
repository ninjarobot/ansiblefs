all: clean build

clean:
	rm -rf **/**/bin
	rm -rf **/**/obj

restore:
	dotnet restore

build: restore
	dotnet build

native-linux:
	dotnet publish -r linux-x64 -c Release

native-macos:
	dotnet publish -r osx-x64 -c Release

test: build
	dotnet test

check: clean test

