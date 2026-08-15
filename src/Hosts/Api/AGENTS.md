# API host

Adapt HTTP input and output, configure dependency injection, and compose the
WineJournal module with PostgreSQL. Product behavior belongs to WineJournal.

Only composition and transport code may live here. The host may depend on the
module; the module must never depend on this host.
