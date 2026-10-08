# ADR-0002 — Persistência do Customer e unicidade do CPF

## Status
Accepted

## Context

O CPF identifica unicamente um cadastro e precisa continuar único mesmo quando requisições concorrentes tentam cadastrar o mesmo valor. A validação também deve persistir a representação canônica do CPF.

## Decision

Usar SQL Server com EF Core e migrations pertencentes ao Customer Service. Normalizar CPF para 11 dígitos ASCII no domínio, consultar duplicidade na Application para resposta antecipada e impor índice único no banco como garantia concorrente. A Infrastructure traduz violações de unicidade da gravação para uma exceção semântica da Application.

## Alternatives Considered

- Confiar somente na consulta prévia da Application.
- Usar apenas a restrição do banco e deixar o erro de persistência vazar para a API.
- Compartilhar a tabela de clientes com outros serviços.

A consulta prévia isolada não evita corrida. A restrição isolada exigiria tratar um erro técnico mais tarde no fluxo. Banco compartilhado aumenta o acoplamento entre serviços.

## Consequences

- A resposta de duplicidade é `409 Conflict`, inclusive em colisões concorrentes.
- Migrations devem preservar o índice único em `Customers.CPF`.
- A tradução atual reconhece os códigos SQL Server de chave única (2601 e 2627); outra estratégia de banco exigirá adaptação na Infrastructure.
- Testes unitários e HTTP usam repositório em memória; a garantia física do índice deve ser validada também contra SQL Server.

## Implementation

`CustomerDbContext` converte o value object para string e define o índice único. `CreateCustomerHandler` faz a consulta antecipada. `CustomerRepository` converte falhas SQL de unicidade em `CustomerCpfAlreadyExistsException`.

## Validation

Build e 27 testes unitários/HTTP e de tradução LINQ passam, incluindo CPF duplicado antes da gravação e conflito no caminho de persistência simulado. As migrations e o snapshot existentes contêm o índice único. Não houve validação de conectividade ou gravação em SQL Server nesta execução.