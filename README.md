# PlusHosting-IPUpdate-Service
[![.NET](https://github.com/cacafaca/PlusHosting-IPUpdate-Service/actions/workflows/dotnet.yml/badge.svg)](https://github.com/cacafaca/PlusHosting-IPUpdate-Service/actions/workflows/dotnet.yml)

## Windows Service to update dynamic IP address on hosting site https://portal.plus.rs
A good way if you dont want to pay for a static IP address to your provider but you want to pay for your domain and get redirected to your IP address. This service is updating IP address every 5 minutes.

Currently it's parsing HTML on pages, but the PlusHosting support API. So, the plan is to relay on API and not on HTML parsing. :)

## Settings
Check out template file "**LoginInfoTemplate.json**" to see how you can configure your instance.
