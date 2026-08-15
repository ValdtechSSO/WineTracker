# WineTracker web host

This Angular 22 host delivers the responsive WineTracker interface. Angular
Material supplies every interactive component. Product rules and persistence
remain in the WineJournal module and are reached through `/api`.

From the repository root:

```bash
npm ci --prefix src/Hosts/Web
npm start --prefix src/Hosts/Web
```

The development server uses `proxy.conf.json` to forward `/api` to
`http://localhost:5080`.

Run the host checks with:

```bash
npm run check --prefix src/Hosts/Web
```
