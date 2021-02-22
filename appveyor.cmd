@setlocal
@pushd %~dp0
@set _C=Release

msbuild -t:restore -p:Configuration=%_C% || exit /b

msbuild -p:Configuration=%_C% || exit /b

@popd
@endlocal