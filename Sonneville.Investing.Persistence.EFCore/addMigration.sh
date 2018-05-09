#!/usr/bin/env bash

dotnet ef --startup-project ../Sonneville.Fidelity.Shell migrations add $1
