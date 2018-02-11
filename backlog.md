# Feature backlog

Currently, this project is little more than a proof-of-concept demo which scrapes some fundamental account/portfolio information from Fidelity's website.

## Long term vision

Ultimately, this project could serve as the plumbing for:
* Automated trading/rebalancing engines
* Portfolio analysis tools
* Statement retrieval and storage

## Mid term goals

Currently, I'm working on enabling a basic rebalancing workflow:
1. Retrieve recent transactions (**complete**)
1. Merge recent transactions with storage of all historical transactions (**in progress**)
1. Calculate current allocation and compare with desired (stored) allocation
1. Calculate orders which would rebalance the allocation
1. Initiate orders on Fidelity's website

## Short term opportunities

Start here if you're interested in contributing to the success of an open-source data collector for Fidelity.com! Please review [the contribution guidelines](https://github.com/SonnevilleJ/Investing/CONTRIBUTING.md) and consider helping out on one of the features listed below:

### Free Continuous Integration

Every project needs a CI server. Because this project works on the .NET Framework and Mono, it works on Windows, Linux, and Mac. Ideally, this project would have a CI environment for each platform, but even one is better than none.

If you would like to contribute by configuring a CI environment, please [submit a pull request](https://github.com/SonnevilleJ/Investing/compare)!

### Transaction Storage

Transaction history is critical for in-depth portfolio analysis, accurate fee/tax calculations, growth projections, and more. Before any of these exciting features can be developed, the application needs a way to store transactions and other types of account data.

The only storage mechanism I have in place is [a rudimentary JSON storage repository](https://github.com/SonnevilleJ/Investing/blob/master/Sonneville.Utilities/Persistence/v2/JsonDataStore.cs). This will work, but a database would be better. As the project is not hosted anywhere (and likely won't be anytime soon, if ever) a local database of some kind would be best.

If you would like to contribute by configuring a database persistence workflow, please [submit a pull request](https://github.com/SonnevilleJ/Investing/compare)!

### CLI Usability

A simple CLI is available in the Sonneville.Fidelity.Shell project. While a `help` command is available, it doesn't provide much documentation. Additionally, the other commands require the user to understand how to interact with them.

If you would like to contribute by improving the command line interface, please [submit a pull request](https://github.com/SonnevilleJ/Investing/compare)!
