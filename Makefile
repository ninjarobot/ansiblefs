all: clean build

clean:
	rm -rf **/**/bin
	rm -rf **/**/obj

restore:
	dotnet restore

build: restore
	dotnet build

test: build
	dotnet test

check: clean test

