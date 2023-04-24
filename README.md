# Foodly API

This is the API for a web application for ordering food built with ASP.net and integrated with Stripe for payment processing. It provides endpoints for managing food items, orders, and payments.

## Installation

To run the application, you will need to have [.NET 7 SDK](https://dotnet.microsoft.com/download) installed on your computer.

Clone the repository to your local machine:

```
git clone https://github.com/thevictormadu/foodlyapi.git
```

Navigate to the root directory and create a `appsetting.json` file with the following contents:

```
STRIPE_SECRET_KEY=your_stripe_secret_key
```

Replace `your_stripe_secret_key` with your Stripe secret key.

Then, run the following command to start the server:

```
dotnet run
```

The application will then be available at `http://localhost:5000`.

