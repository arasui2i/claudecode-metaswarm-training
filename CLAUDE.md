# CRM Constitution

## Backend Standards

Technology:

* ASP.NET Core 9
* SQL Server
* EF Core with Code first approach

Architecture:

* Clean Architecture
* CQRS + MediatR

Security:

* JWT Authentication
* Role Based Authorization

Testing:

* nUnit

Coding Standards:

* Async/Await
* FluentValidation
* Repository Pattern

Naming:

* Commands end with Command
* Queries end with Query

## Frontend Standards

Framework: React
Language: TypeScript

UI:

* Material UI

State Management:

* React Query

Forms:

* React Hook Form

Testing:

* Vitest
* React Testing Library

## Project Structure

```
specs/                    # Feature specs and plans (source of truth before coding)
CRM.Domain/               # Domain entities
CRM.Infrastructure/       # EF Core, repositories, JWT
CRM.Application/          # CQRS commands/queries (MediatR)
CRM.API/                  # ASP.NET controllers
CRM.Tests/                # nUnit test project
src/                      # React frontend
.metaswarm/               # Metaswarm config and knowledge base
.github/workflows/        # CI — runs on every PR to main
```

## Commands

### Backend
```bash
dotnet build
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend (from `src/`)
```bash
npm install
npm run typecheck
npm run lint
npm test -- --run
npm test -- --coverage --run
```

## Quality Gates

- **Coverage**: 80% minimum (backend and frontend)
- **TDD**: Tests written before implementation
- **No `any`**: TypeScript strict; use `as never` for DI wiring in tests
- **No `--no-verify`**: Never bypass git hooks
- **Naming**: Commands end with `Command`, Queries end with `Query`

## Metaswarm Agent Coordination

1. `/metaswarm:start` — begin a new feature from a spec
2. `plan-review-gate` — 3 independent reviewers must approve the plan before coding
3. `design-review-gate` — PM, Architect, Designer, Security, CTO approve design
4. `orchestrated-execution` — IMPLEMENT → VALIDATE → ADVERSARIAL REVIEW → COMMIT
5. `/metaswarm:pr-shepherd` — monitors PR through CI and merge

### Agent Roles
- **swarm-coordinator**: top-level orchestrator across multiple issues
- **issue-orchestrator**: manages one GitHub Issue end-to-end
- **researcher**: codebase exploration and prior art research
- **architect**: creates implementation plan
- **cto**: reviews and approves the plan
- **coder**: TDD implementation (red-green-refactor)
- **code-review**: internal review before PR
- **test-automator**: coverage analysis and test gap filling
- **security-auditor**: OWASP / vulnerability checks
- **pr-shepherd**: CI monitoring and merge management


<!-- BEGIN BEADS INTEGRATION v:1 profile:minimal hash:7510c1e2 -->
## Beads Issue Tracker

This project uses **bd (beads)** for issue tracking. Run `bd prime` to see full workflow context and commands.

### Quick Reference

```bash
bd ready              # Find available work
bd show <id>          # View issue details
bd update <id> --claim  # Claim work
bd close <id>         # Complete work
```

### Rules

- Use `bd` for ALL task tracking — do NOT use TodoWrite, TaskCreate, or markdown TODO lists
- Run `bd prime` for detailed command reference and session close protocol
- Use `bd remember` for persistent knowledge — do NOT use MEMORY.md files

**Architecture in one line:** issues live in a local Dolt DB; sync uses `refs/dolt/data` on your git remote; `.beads/issues.jsonl` is a passive export. See https://github.com/gastownhall/beads/blob/main/docs/SYNC_CONCEPTS.md for details and anti-patterns.

## Session Completion

**When ending a work session**, you MUST complete ALL steps below. Work is NOT complete until `git push` succeeds.

**MANDATORY WORKFLOW:**

1. **File issues for remaining work** - Create issues for anything that needs follow-up
2. **Run quality gates** (if code changed) - Tests, linters, builds
3. **Update issue status** - Close finished work, update in-progress items
4. **PUSH TO REMOTE** - This is MANDATORY:
   ```bash
   git pull --rebase
   git push
   git status  # MUST show "up to date with origin"
   ```
5. **Clean up** - Clear stashes, prune remote branches
6. **Verify** - All changes committed AND pushed
7. **Hand off** - Provide context for next session

**CRITICAL RULES:**
- Work is NOT complete until `git push` succeeds
- NEVER stop before pushing - that leaves work stranded locally
- NEVER say "ready to push when you are" - YOU must push
- If push fails, resolve and retry until it succeeds
<!-- END BEADS INTEGRATION -->
