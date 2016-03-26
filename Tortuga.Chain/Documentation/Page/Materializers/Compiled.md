# Compiled Object/Collection Materializers

Compiled materializers require the `Tortuga.Chain.CompiledMaterializers` package. 

* `.Compiled.ToObect<TObject>`
* `.Compiled.ToCollection<TObject>`
* `.Compiled.ToCollection<TCollection, TObject>`

## Options

The `ToObject` materializer supports the `RowOptions` enumeration.

The `ToCollection` materializer returns a `List<TCollection>` by default. You can override the collection type with any `ICollection<TObject>` so long as it isn’t read-only.

## Limitations

Object/Collection materializers require that the Decompose attribute be applied correctly. The materializer needs to walk the entire object graph, and if there are any cycles represented by decomposed properties then a stack overflow exception will occur.

Object/Collection materializers can only populate public properties. It cannot set fields or non-public properties.

Compiled Object/Collection materializers only support public types. Because it does not use reflection, it cannot sidestep the visibility restrictions like the non-compiled versions can.

Compiled Object/Collection materializers do not support generic types (other than Nullable<T>). This is not a design limitation and will be fixed in a later version. https://github.com/docevaad/Chain/issues/64

Compiled Object/Collection materializers do not support nested types. This is not a design limitation and will be fixed in a later version. https://github.com/docevaad/Chain/issues/63

## Internals

Compiled Object/Collection materializers use CS Script to generate the population methods.

## Roadmap

Eventually we would like to support non-default constructors so that we can populate immutable objects.

