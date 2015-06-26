﻿using System;
using Sonneville.FidelityWebDriver.Pages;

namespace Sonneville.FidelityWebDriver
{
    public interface ISiteNavigator : IDisposable
    {
        IHomePage GoToHomePage();
        ILoginPage GoToLoginPage();
        IActivityPage GoToActivityPage();
    }
}