# Dictionary Materializers

* `.ToDictionary<TKey, TObject>`
* `.ToDictionary<TKey, TObject,TDictionary>`
* `.ToImmutableDictionary<TKey, TObject>`

## Options

These materializers supports the `DictionaryOptions` enumeration. 

The `ToDictionary` materializer returns a `Dictionary<TKey, TObject>` by default. You can override the collection type with any `IDictionary<TObject>` so long as it isn’t read-only.

Keys can be generated from a column name or using a `Func<TObject, TKey>`. In the former case, the key column doesn't need to be a property on the object.

The normal behavor is to throw an exception if a duplicate key is found. Using the `DiscardDuplicates` option, duplicate values are ignored. (Currently the first value is overriden by the second value. This is an implementation detail and should not be relied upon.)

## Capabilities and Limitations

Dictionary materializers have the same capabilities and limitaions as Object/Collection materializers, including non-default constructor support.

## SQL Generation

As per above, mapped properties on the object (and child properties on a decomposed property) will be requested.

In a non-default constructor is chosen, then SQL will only be generated for the columns that match parameter names in the constructor.

Additionally, the column needed for key generation will be included.

## Internals

Dictionary materializers use reflection to instantiate and populate the object.

## Roadmap

