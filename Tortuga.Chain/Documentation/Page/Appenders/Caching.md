# Caching Appenders

* `.Cache(...)`
* `.CacheAllItems(...)`
* `.InvalidateCache(...)`
* `.ReadOrCache(...)`

## Cache Keys

Cache keys can be provided as a string or a function. In the latter case, the function accepts the result of the chain and returns a string.

`.CacheAllItems(...)` is a special case. It only operates on lists of objects and generates a cache key for each object seperately.

## Internals

The caching appenders use the data source to access a cache. Currently that data source is hard-coded to use .NET's built-in `MemoryCache`.

## Roadmap

The current plan is to allow the caching framework to be swapped out. 
