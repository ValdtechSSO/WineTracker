# Global invariants

## Wine history

1. Every consumption references exactly one existing wine.
2. Repeated consumption of the same wine adds history; it does not overwrite a
   previous event.
3. A wine owns its producer, label, optional vintage, type, and optional region.

## Reorder intent

1. Reorder intent is one of `yes`, `no`, or `undecided`.
2. Reorder intent is recorded explicitly and is not inferred from rating or notes.

## Operational boundary

1. PostgreSQL is the authoritative store for wines and consumption events.
2. Initial operation is local and single-user.
3. Personal wine history must not be sent to an external service without explicit authorization.
