# Technical Architecture & Modern Dev Practices

## 1. Next.js App Router Optimization
- **Server Component Default:** All UI views are React Server Components (RSC) by default to maximize load velocity and reduce public client-side JavaScript.
- **Granular Hydration:** Only implement the `"use client"` boundary directive at the absolute tips of your component tree where explicit user state hooks (`useState`, `useEffect`) are mandatory.
- **Authorized Server Actions:** When utilizing Next.js Server Actions for processing changes, always enforce explicit session validation checks inside the action scope before modifying database entities.

## 2. TypeScript Type Integrity
- **Strict Typing Rules:** Turn off fallback loose compilation parameters. Avoid using `any`. If a property payload is dynamic or unknown, utilize `unknown` paired with strict type guard functions.
- **Props Definitions:** Every functional component must map its input signatures to an explicitly defined TypeScript `interface` or `type` block.

## 3. Tech-Luxury Minimalist UI Directives
- **Visual Design Benchmark:** Elite, minimalist, and deeply intentional. Emphasize crisp typographic weights, generous spacing, structural alignment, and discrete micro-animations.
- **No Visual Bloat:** Do not generate loud background color gradients, heavy separating borders, or random decorative layout additions. Rely strictly on neutral color scales, structural clarity, and Tailwind CSS utility classes. Do not drop raw inline `style={{...}}` blocks into code.
