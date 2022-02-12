## 2.3.0 (February 12, 2022)

### Fixes
- Fix [#15 Import-Module fails](https://github.com/v2kiran/PSLiteDB/issues/15)
- Fix dates were not being converted from strings in Windows PowerShell 5.1

### Internal Updates
- Fix pester tests

## 2.2.0 (August 12, 2021)

### Fixes
- Fix [#10 String is always converted to DateTime under certain conditions](https://github.com/v2kiran/PSLiteDB/issues/10)
- Fix [#11 Converting to string error.](https://github.com/v2kiran/PSLiteDB/issues/11)

### Enhancements
- Updated litedb to the latest available version 5.0.11
## 2.1.0 (October 17, 2020)

### Fixes
- Fix [#7 Pipeline input for Updating docs](https://github.com/v2kiran/PSLiteDB/issues/7)

### Enhancements
- Added help for all cmdlets.
- added de-serialization of nested BSON docuemnts to PSObjects.
- added pester v5 based tests
- updated readme with an example that would work on any OS.
- Updated litedb to the latest available version 5.0.9