# Serverless Edge Architecture & Cloud Infrastructure

## 1. AWS Serverless & v0 Edge Compatibility
- **Stateless Operation:** All runtime logic must remain completely stateless. Do not write dynamic session states or cache persistent dependencies to local file system buffers (`/tmp` or internal memory matrices) that assume cross-request persistence.
- **Next.js Edge Optimization:** Ensure all dynamic routes, server-side data fetches, and background functions compile cleanly under standard Vercel Edge Runtime or AWS Lambda limitations. Avoid using Node.js native library components (`fs`, `path`, `child_process`) inside files handling client-facing backend responses.
- **CloudFront Cache Integrity:** Ensure API endpoints destined to live behind an active CloudFront CDN explicitly define unique caching headers (`Cache-Control`) to block cross-browser session leakage.

## 2. NoSQL & DynamoDB Data Modeling
- **Single-Table Architecture Default:** Structure database records through unified partition keys (`PK`) and sort keys (`SK`) to satisfy specific user access patterns cleanly. Completely avoid slow, intensive table scanning loops.
- **Missing State Resiliency:** When reading documents from DynamoDB, always handle data fallback properties gracefully, accounting for values that may be entirely missing or conditionally structured.
