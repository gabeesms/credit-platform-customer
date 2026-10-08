# ADR-0001 — Limites dos serviços e ownership dos dados

## Status
Accepted

## Context

A plataforma é composta por serviços Customer, Credit e Card. O Customer Service precisa ser concluído sem criar dependência direta de bancos pertencentes aos demais serviços.

## Decision

Customer Service é responsável pelos dados cadastrais e pelo seu próprio banco. Cada serviço da plataforma deve ser dono de seus dados; consumidores externos não acessam diretamente esse banco. A implementação atual contém apenas Customer Service.

## Alternatives Considered

- Banco compartilhado entre serviços, com acesso direto às tabelas.
- Um serviço central de dados acessado por todos os demais.
- Persistência independente por serviço com integração por contratos explícitos.

Foi escolhida a persistência independente por serviço para reduzir acoplamento e permitir que cada serviço evolua seu modelo sem impor mudanças aos demais.

## Consequences

- Customer Service controla o esquema e as migrations de `CustomerServiceDb`.
- Futuros consumidores não devem importar entidades de domínio nem criar foreign keys entre bancos.
- Integrações futuras exigirão contratos e decisões próprias; não são implementadas por este ADR.

## Implementation

O repositório EF Core e as migrations Customer ficam em `CustomerService.Infrastructure`. Credit e Card permanecem fora deste projeto e não foram implementados nesta etapa.

## Validation

Build e testes do Customer Service passam. O isolamento foi verificado na estrutura de projetos e na ausência de referências ou conexões a bancos de outros serviços. Não foi executado teste de integração com SQL Server.