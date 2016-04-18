# Object/Collection Materializers

* `.ToObect<TObject>`
* `.ToCollection<TObject>`
* `.ToCollection<TCollection, TObject>`

## Options

The `ToObject` materializer supports the `RowOptions` enumeration.

The `ToCollection` materializer returns a `List<TCollection>` by default. You can override the collection type with any `ICollection<TObject>` so long as it isn’t read-only.

## Capabilities

Object/Collection materializers can populate non-public types.

Object/Collection materializers honor the Column attribute, which changes which query result set column the property is mapped to.

Object/Collection materializers honor the `NotMapped` attribute, which prevent the property from being mapped.

Object/Collection materializers honor the `Decompose` attribute. This allows properties on the child object when they match columns in the query result set.

If the desried object implementes `IChangeTracking`, then `AcceptChanges()` will be called automatically.

## Non-default Constructors

If you use the `InferConstructor` option or explicit provide a constuctor signature, the behavior changes. Instead of setting properties, the indicated constructor will be called.

The `Decompose` attribute doesn't apply when using a non-default constructor.

## SQL Generation

As per above, mapped properties on the object (and child properties on a decomposed property) will be requested.

In a non-default constructor is chosen, then SQL will only be generated for the columns that match parameter names in the constructor.

## Limitations

Object/Collection materializers require that the `Decompose` attribute be applied correctly. The materializer needs to walk the entire object graph, and if there are any cycles represented by decomposed properties then a stack overflow exception will occur.

Object/Collection materializers can only populate public properties. It cannot set fields or non-public properties (unless you use a constructor).

## Internals

Object/Collection materializers use reflection to instantiate and populate the object.

## Roadmap

