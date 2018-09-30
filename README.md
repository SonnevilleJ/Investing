# Project Overview
A C# library with some basic domain objects for applications interested in managing financial investments. A command line interface (CLI) is also available, see the Sonneville.Fidelity.Shell project in this repository.

## Current build status
* MSBuild: None! - Please see the [feature backlog](https://github.com/SonnevilleJ/Investing/tree/master/backlog.md) if you'd like to contribute a CI configuration for this project.
* Mono: None! - Please see the [feature backlog](https://github.com/SonnevilleJ/Investing/tree/master/backlog.md) if you'd like to contribute a CI configuration for this project.

## Integration tests
This project requires a PostgreSQL database with the [specified connection details](https://github.com/SonnevilleJ/Investing/blob/data-storage/Sonneville.Investing.Persistence.IntegrationTest/IntegrationTestConnection.cs). To use Docker for Postgres/pgAdmin, run the following command(s):

PosgreSQL: `docker run --name=Sonneville-Investing-Postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=pwd -e POSTGRES_DB=investing -p 6543:5432 -d postgres:9.6`
pgAdmin 4: `docker run --name=Sonneville-Investing-pgAdmin -e PGADMIN_DEFAULT_EMAIL=postgres -e PGADMIN_DEFAULT_PASSWORD=pwd -p 8080:80 -d dpage/pgadmin4`

## Demo app
The demo app will use the specified credentials to log into the [www.Fidelity.com](https://www.fidelity.com) website. Once logged in, it will print to the console some basic account info as well as the most recent transactions.

### Launching the demo
1. Complie the solution and run Sonneville.FidelityWebDriver.Demo.exe, typing `demo` at the prompt, including any of the below parameters:
```
  -u, --username=VALUE       the username to use when logging into Fidelity.
  -p, --password=VALUE       the password to use when logging into Fidelity.
  -s, --save                 indicates options should be persisted to demo.ini file.
  -h, --help                 shows this message and exits.
```
If the `--save` argument is given, settings are stored to `FidelityWebDriver.Demo.json` allowing future demo executions to omit the `-u` and `-p` parameters. NOTE: This is intended purely as a convenience and is not a secure storage mechanism. Please consider the risks.

## Fidelity Web Driver
This project includes a library (Sonneville.Fidelity.WebDriver) as a wrapper around the [Selenium WebDriver](http://www.seleniumhq.org/projects/webdriver). This library is used to perform semantic interaction with the Fidelity website.

### Managers - these managers work for you!
To use the FidelityWebDriver library, identify the IManager implementation which provides the functionality you'd like to consume. Instantiate the manager and required classes using your choice of IWebDriver implementation. An IOC library like [Ninject](http://www.ninject.org) is recommended.

Available managers   | Description |
-------------------- | ----------------------------------------------------
Login Manager        | manages login state for the Fidelity.com website
Positions Manager    | retrieves current positions
Transactions Manager | retrieves previous transactions

## Troubleshooting
This project should always be functional if built from the master branch. Logging is achieved via [Apache log4net](https://logging.apache.org/log4net). Log files for the demo app can be retrieved from the following location:

Platform  | Location
----------|-------------------------------------------------------
Windows   | `%LocalAppData%\John Sonneville\Fidelity Shell\`
Mac/Linux | `~/.local/share/John\ Sonneville/Fidelity Shell/`

Thanks for using this product! Please [create an issue](https://github.com/SonnevilleJ/Investing/issues/new) with any bugs!
