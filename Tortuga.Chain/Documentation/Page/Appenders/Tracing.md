# Tracing Appenders

The tracing appenders are usually used for debugging purposes. For logging, the events on DataSource are more useful.

* `.WithTracing`
* `.WithTracingToDebug`
* `.WithTracingToConsole`

## Arguments

The generic `.WithTracing` appender accepts a `TextWriter`, while the others 

## Internals

These appenders listen for the OnCommandBuilt event.

## Roadmap


