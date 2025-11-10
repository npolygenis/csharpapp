# C# Accepted Assessment app

An application for C# (.net) knowledge assessment

## Description

This is a web application that interacts with a 3nd party service (<https://fakeapi.platzi.com>/<https://api.escuelajs.co>) and serves data

We have to do some code refactoring and implement some new features

## Code refactoring

The ProductsService refactored to use the IHttpClientFactory (The ProductsService currently creates a new HttpClient instance, which is inefficient and can lead to performance issues)

## New features

**#1**

Right now only the **getAll** method supported for **products**

Implement the "get one product" feature

Implement the "create product" feature

**#2**

implement  categories

**#3**

implement JWT auth and support it,use the credentials provided to appsettings.json file.

**#4**

Created a middleware to measure and log the performance of the requests.

## Implementation

* Try to understand and keep the architectural approach.
* Added unit testing.
* Add docker support.
* Using CQRS pattern will be considered as a strong plus.
* The attached collections (postman/insomnia) will help you with the requests.
