   # Finance Tracker AI Agent - Architecture Document

   ## 1. System Overview
   - **Core Problem:** AI-driven finance buddy helping users with budgeting, investment advice, and expense tracking
   - **Approach:** Design MVP for thousands of users, identify breaking points at 10k → 100k → 1M scale
   - **Primary Cost Driver:** LLM API calls (OpenAI)
   
   ## 2. Scaling Phases & Breaking Points

   ### Phase 1: MVP (1k-10k users)
   - **Architecture:** Monolithic, simple caching
   - **What works:** Direct DB queries, basic rate limiting
   - **What breaks at 10k:**
     - DB query latency (unindexed queries slow down)
     - LLM costs spike (no intelligent caching)
     - Cache invalidation becomes messy (no clear strategy)
   
   ### Phase 2: Early Growth (10k-100k users)
   - **What changes:** Separate cache layer (Redis), query optimization, smarter AI routing
   - **What breaks at 100k:**
     - Single DB instance becomes bottleneck
     - LLM throughput limits
     - Need for background job processing
   
   ### Phase 3: Scale (100k-1M users)
   - **What changes:** Database sharding, queue system (Bull/RabbitMQ), AI service isolation
   - **Key insight:** LLM becomes a service dependency, not inline

   ## 3. MVP Cache Strategy (Phase 1)
   - **Cache TTL:**
     - User portfolio data: 1 hour (not mission-critical freshness)
     - Expense summaries: 30 minutes (more volatile)
     - AI responses: 24 hours (user data rarely changes that fast)
   - **Invalidation Rules:**
     - Portfolio cache: Expires on TTL or explicit user sync
     - AI responses: Expires on TTL or when user adds new data
   
   ## 4. MVP Rate Limiting (Phase 1)
   - **Per-user limits:** 5 AI calls per day (conservative, cost-conscious)
   - **Global limits:** Start monitoring, scale as needed
   - **Enforcement point:** Middleware (reject before hitting AI service)
   
   ## 5. Hybrid AI Invocation (Phase 1)
   - **When AI is called:**
     - User explicitly requests advice ("Should I invest in X?")
     - Scheduled batch jobs (daily budget insights)
   - **When AI is NOT called:**
     - Dashboard data retrieval (served from cache)
     - Expense logging (just storage, no analysis)

   ## 6. Database Strategy
   - **Phase 1:** Single PostgreSQL instance, proper indexing on user_id, expense_date
   - **Phase 2:** Read replicas for reporting queries
   - **Phase 3:** Sharding by user_id (if needed)