# Listas Pessoais — Resumo da feature

## TL;DR

Adicionei o CRUD básico das listas pessoais (Watchlist / Watched / Favorites + Custom). Reutiliza a infra de autenticação JWT que já existia. Sem migrations — os modelos já estavam no `AppDbContext` e o `DbSeeder` já cria as 3 listas predefinidas ao registar um utilizador.

---

## Endpoints expostos

Todos exigem JWT (`[Authorize]` no controller). Cada operação valida que a lista pertence ao utilizador autenticado.

| Verbo | Rota | Resposta sucesso | Erros |
|---|---|---|---|
| `GET` | `/api/listas` | `200` lista das listas do utilizador (com items + filmes incluídos) | `401` sem token |
| `POST` | `/api/listas` | `201 Created` lista nova | `400` nome vazio |
| `POST` | `/api/listas/{id}/filmes/{filmeId}` | `200 OK` item adicionado | `404` lista/filme inexistente, `403` lista de outro utilizador, `409` filme já está na lista |
| `DELETE` | `/api/listas/{id}/filmes/{filmeId}` | `204 No Content` | `404`, `403` |

### Body do `POST /api/listas`

```json
{
  "name": "Para rever",
  "description": "Filmes que quero rever",
  "tipo": 0,
  "isPublic": false
}
```

---

## Tipos de lista (`TipoListaPessoal` enum)

| Valor | Nome interno | `TipoNome` (em PT-PT) |
|---|---|---|
| `0` | Custom | "Personalizada" |
| `1` | Watchlist | "Quero ver" |
| `2` | Watched | "Vistos" |
| `3` | Favorites | "Favoritos" |

As 3 listas predefinidas (Watchlist/Watched/Favorites) são criadas automaticamente no `POST /api/auth/register` — ver [AuthService.cs:77-100](OnlineCinemaFestival.Api/Services/AuthService.cs#L77-L100).

---

## Arquitetura

Seguiu o fluxo em camadas do projeto: **Controller → Service → Repository → AppDbContext**.

```
Controllers/ListasController.cs
        ↓ (injeta)
Services/IListaPessoalService → Services/ListaPessoalService
        ↓ (injeta)
Repositories/IListaPessoalRepository → Repositories/ListaPessoalRepository
        ↓ (injeta)
Data/AppDbContext  (DbSet<ListaPessoal>, DbSet<ListaPessoalItem> já existiam)
```

DTOs nunca expõem entidades EF diretamente — mapeamento centralizado em [Mappers/ListaPessoalMapper.cs](OnlineCinemaFestival.Api/Mappers/ListaPessoalMapper.cs).

---

## Ficheiros criados / alterados

**Novos:**
- [DTOs/ListaPessoalCreateDto.cs](OnlineCinemaFestival.Api/DTOs/ListaPessoalCreateDto.cs)
- [DTOs/ListaPessoalReadDto.cs](OnlineCinemaFestival.Api/DTOs/ListaPessoalReadDto.cs)
- [DTOs/ListaPessoalItemReadDto.cs](OnlineCinemaFestival.Api/DTOs/ListaPessoalItemReadDto.cs)
- [Mappers/ListaPessoalMapper.cs](OnlineCinemaFestival.Api/Mappers/ListaPessoalMapper.cs)
- [Repositories/IListaPessoalRepository.cs](OnlineCinemaFestival.Api/Repositories/IListaPessoalRepository.cs)
- [Repositories/ListaPessoalRepository.cs](OnlineCinemaFestival.Api/Repositories/ListaPessoalRepository.cs)
- [Services/IListaPessoalService.cs](OnlineCinemaFestival.Api/Services/IListaPessoalService.cs)
- [Services/ListaPessoalService.cs](OnlineCinemaFestival.Api/Services/ListaPessoalService.cs)
- [Controllers/ListasController.cs](OnlineCinemaFestival.Api/Controllers/ListasController.cs)

**Alterado:**
- [Program.cs](OnlineCinemaFestival.Api/Program.cs) — registo do repository + service no DI, e config nova do Swagger (ver secção "Bónus" mais abaixo)

**Não tocado:** Modelos `ListaPessoal`, `ListaPessoalItem`, enum `TipoListaPessoal`, `AppDbContext` (configuração já existia), `DbSeeder` (já criava as listas predefinidas).

---

## Decisões tomadas

1. **`GET /api/listas` devolve só as listas do utilizador autenticado**, não as públicas de toda a gente. O `IsPublic` no modelo serve outro caso de uso (perfil público) que pode ser exposto noutro endpoint mais à frente.

2. **Lê `userId` do JWT no Controller, passa para o Service.** Mesmo padrão do `ProfilesController.GetCurrentUserId()` — usa `User.FindFirstValue(ClaimTypes.NameIdentifier)`. O Service não conhece HTTP nem `ClaimsPrincipal`.

3. **Verificação de posse centralizada no Service.** `GarantirAcessoLista(utilizadorId, listaId)` carrega a lista, valida que existe e que pertence ao utilizador, devolve a entidade. Cada operação que mexe numa lista usa este método.

4. **Status codes específicos por tipo de erro:**
   - `KeyNotFoundException` → `404`
   - `UnauthorizedAccessException` → `403`
   - `InvalidOperationException` → `409` (duplicação)
   - `ArgumentException` → `400`

5. **Item duplicado dá `409 Conflict`**, não 400. A chave composta `(ListaPessoalId, FilmeId)` no `AppDbContext.OnModelCreating` já impede isto a nível de BD, mas o service deteta antes para devolver mensagem útil em vez de uma `DbUpdateException`.

6. **`UpdatedAt` da lista é atualizado em cada add/remove** — útil para ordenação por "modificado recentemente" no frontend.

7. **`GetByUtilizadorAsync` usa `Include + ThenInclude`** para trazer também os `Filme` de cada item numa única query. `AsNoTracking()` porque o GET é só leitura.

8. **DTOs `Read` em PT mas propriedades em EN/PT misturadas** — segui exatamente o que já estava no modelo (`Name`, `Description`, `IsPublic` em EN, herdados do estilo dos `Festival` DTOs). O `TipoNome` é o único campo "amigável" em PT, para o frontend mostrar sem ter de saber o enum.

9. **Sem migration.** Os DbSets `ListasPessoais` e `ListaPessoalItems` já estavam mapeados — verificar [AppDbContext.cs:27-28, 117-137](OnlineCinemaFestival.Api/Data/AppDbContext.cs#L27).

---

## Bónus: Swagger agora tem botão "Authorize"

O `AddSwaggerGen()` estava sem config de segurança, o que tornava impossível testar endpoints `[Authorize]` pelo Swagger UI. Atualizei em [Program.cs](OnlineCinemaFestival.Api/Program.cs) para incluir `OpenApiSecurityScheme` (Bearer JWT).

**Detalhe técnico:** o Swashbuckle 10 + Microsoft.OpenApi 2.x mudou a API. O `OpenApiSecuritySchemeReference` precisa do `OpenApiDocument` no construtor para serializar a `$ref` corretamente — usar a lambda:

```csharp
options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
{
    { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() },
});
```

Sem o `doc`, o `swagger.json` gera `security: [{}]` vazio e o botão não injeta o header.

---

## Como testar (Swagger)

API e Swagger:

```bash
cd OnlineCinemaFestival.Api
dotnet run
# Abrir http://localhost:5152/swagger
```

Fluxo:

1. **`POST /api/auth/register`** — cria conta nova e devolve já o JWT na resposta:
   ```json
   { "name": "Teste", "email": "teste@exemplo.pt", "password": "123456", "nationality": "Portugal" }
   ```
   Copia o valor de `token` da resposta.

2. **Botão "Authorize"** (topo direito da página) → cola o token (**sem** `Bearer ` à frente) → "Authorize" → "Close".

3. **`GET /api/listas`** → `200 OK` com as 3 listas predefinidas (Quero ver / Vistos / Favoritos).

4. **`GET /api/catalogo`** (anónimo) → escolhe um `id` de filme.

5. **`POST /api/listas/{idListaFavoritos}/filmes/{idFilme}`** → `200 OK` com o item.

6. **`GET /api/listas` outra vez** — a lista "Favoritos" deve agora ter o filme em `items` e `totalFilmes: 1`.

7. **`POST` repetido com os mesmos ids** → `409 Conflict` "O filme já se encontra nesta lista."

8. **`DELETE /api/listas/{idLista}/filmes/{idFilme}`** → `204 No Content`.

9. **`POST /api/listas`** para criar lista personalizada:
   ```json
   { "name": "Para rever", "description": "", "tipo": 0, "isPublic": false }
   ```
   `tipo: 0` = Custom. Resposta `201 Created`.

### Como testar via cURL (alternativa)

```bash
# 1. Registo (devolve token)
TOKEN=$(curl -s -X POST http://localhost:5152/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Teste","email":"teste@exemplo.pt","password":"123456","nationality":"PT"}' \
  | jq -r .token)

# 2. Listar as minhas listas
curl http://localhost:5152/api/listas -H "Authorization: Bearer $TOKEN"

# 3. Adicionar filme à lista 3
curl -X POST http://localhost:5152/api/listas/3/filmes/1 \
  -H "Authorization: Bearer $TOKEN"
```

---

## Para o frontend

Quando integrarem no cliente Blazor, o `ManipuladorTokenHttp` já injeta o `Bearer` automaticamente, basta criar um `ListasService` no padrão dos outros (`FestivalService`, `PerfilService` em [OnlineCinemaFestival.Client/Services/](OnlineCinemaFestival.Client/Services/)). DTOs cliente espelham exatamente os `ReadDto` (mesmos nomes, mesmos tipos).

Exemplos de chamadas:

```csharp
public Task<List<ListaPessoalDto>> GetMinhasListasAsync()
    => _http.GetFromJsonAsync<List<ListaPessoalDto>>("api/listas");

public Task AdicionarFilmeAsync(int listaId, int filmeId)
    => _http.PostAsync($"api/listas/{listaId}/filmes/{filmeId}", null);

public Task RemoverFilmeAsync(int listaId, int filmeId)
    => _http.DeleteAsync($"api/listas/{listaId}/filmes/{filmeId}");
```

Sugestão de UX: botão "+ Quero ver" / "★ Favoritos" no `FilmeCard` ou na página de `DetalhesFilme`, e uma página `/listas` que mostra os 3 conjuntos do utilizador autenticado.

---

