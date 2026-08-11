# MCP Context and Token Usage

Principle:
The MCP server/package size itself does not determine LLM token usage.
The content returned to and included in the LLM context does.

Guidelines:
- search before retrieve
- return identifiers and summaries first
- fetch only relevant files/ranges
- paginate large results
- avoid entire repository dumps
- avoid large binary or generated files
- summarize large issue/PR sets before deeper fetches

Recommended pattern:
Search -> shortlist -> retrieve exact items -> reason -> fetch more only if needed.
