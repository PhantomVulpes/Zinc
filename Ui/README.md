# Zinc UI

Vue + TypeScript Frontend for Zinc with Tailwind CSS and PrimeVue

## Tech Stack

- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript  
- **Vite** - Fast build tool and dev server
- **Vue Router** - Official router for Vue.js
- **Tailwind CSS** - Utility-first CSS framework
- **PrimeVue** - Rich UI component library (unstyled mode with Tailwind)
- **PrimeIcons** - Icon library

## Development

```bash
npm install
npm run dev
```

This will start the development server on http://localhost:3000

## Build

```bash
npm run build
```

This builds the production-ready static files to the `dist/` folder.

## Preview

```bash
npm run preview
```

Preview the production build locally.

## API Proxy

The development server is configured to proxy API requests to `http://localhost:60000`. All requests to `/api/*` will be forwarded to the backend API.

## PrimeVue Configuration

PrimeVue is configured in unstyled mode and styled with Tailwind CSS using the `tailwindcss-primeui` preset. This provides a fully customizable component library that integrates seamlessly with Tailwind utilities.
