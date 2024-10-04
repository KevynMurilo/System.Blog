# **Documentação Completa do Sistema de Blog**

## **Regras de Negócio**

### **1. Regras de Acesso (Roles)**

- **Admin**:
  - Pode criar, editar, excluir posts.
  - Pode criar, editar e excluir comentários.
  - Pode gerenciar categorias, tags, e usuários.

- **Escritor (Writer)**:
  - Pode criar e editar posts.
  - Pode editar e excluir **seus próprios comentários**.
  - Não pode editar ou excluir comentários de outros usuários.
  - Não pode gerenciar categorias, tags ou usuários.

- **Leitor (Reader)**:
  - Pode visualizar posts e comentários.
  - Pode comentar em posts, mas não pode criar ou editar posts.
  - Pode editar e excluir **seus próprios comentários**.
  - Não pode editar ou excluir comentários de outros.

### **2. Criação de Post**

- O **usuário deve estar logado** para criar um post.
- O **UserId** será extraído do token JWT enviado no cabeçalho da requisição.
- Apenas **Admins** e **Escritores** poderão criar posts. **Leitores** não terão permissão para criar posts.
- Caso o usuário não esteja autenticado ou não tenha a permissão necessária, será retornado um erro 401 (Unauthorized).

### **3. Edição e Exclusão de Post**

- Apenas o **autor do post** ou um **admin** poderão editar ou excluir um post.
- A validação será feita comparando o **UserId** extraído do token JWT com o **UserId** do autor do post armazenado na tabela **Posts**.
- Se o usuário não for o autor ou um administrador, será retornado um erro 403 (Forbidden).

### **4. Criação de Comentário**

- O **usuário deve estar logado** para criar um comentário.
- O **UserId** será extraído do token JWT na requisição.
- A criação de comentários é permitida para **Leitores**, **Escritores**, e **Admins**.
- Caso o usuário não esteja autenticado, será retornado um erro 401 (Unauthorized).
- **Comentário de Comentário** (Respostas a Comentários):
  - Um comentário pode ter respostas.
  - Para criar uma resposta, o usuário deve estar autenticado e incluir o **`ParentCommentId`** no corpo da requisição, referenciando o comentário original.

### **5. Edição e Exclusão de Comentário**

- **Somente o autor do comentário** ou um **Admin** poderá editar ou excluir o comentário.
  - **Escritores** não poderão editar ou excluir os comentários de outros usuários.
  - **Leitores** não poderão editar ou excluir os comentários de outros usuários.
  
### **6. Acesso aos Posts e Comentários**

- **Todos** (logados ou não) podem visualizar posts e seus respectivos comentários.
- Apenas usuários autenticados (**Admins**, **Escritores**, e **Leitores**) podem criar posts ou comentários.

### **7. Curtidas nos Posts**

- Os usuários podem **curtir** os posts.
- **Curtidas** serão associadas ao **UserId** e ao **PostId**.
- Um usuário pode curtir um post apenas uma vez. Caso o usuário já tenha curtido, ele pode desfazer a curtida.
- Curtidas serão armazenadas em uma tabela separada.

---

## **Modelagem de Dados**

### **1. Tabela: `Users` (Usuários)**

| Coluna         | Tipo           | Restrições                     |
|----------------|----------------|--------------------------------|
| `UserId`       | UUID            | PRIMARY KEY    |
| `Name`         | VARCHAR(255)   | NOT NULL                       |
| `Email`        | VARCHAR(255)   | NOT NULL, UNIQUE               |
| `PasswordHash`        | VARCHAR(255)   | NOT NULL                       |
| `CreatedDate` | DATETIME       | NOT NULL                       |
| `Role`         | ENUM('Admin', 'Writer', 'Reader') | NOT NULL, DEFAULT 'Reader' |
| `PhotoPath`    | VARCHAR(255)           | NULL                            |

### **2. Tabela: `Category` (Categoria)**

| Coluna          | Tipo           | Restrições                     |
|-----------------|----------------|--------------------------------|
| `CategoryId`    | UUID            | PRIMARY KEY    |
| `Name`          | VARCHAR(255)   | NOT NULL, UNIQUE, UPPERCASE    |
| `PhotoPath`    | VARCHAR(255)           | NULL                            |

### **3. Tabela: `Post` (Artigo)**

| Coluna           | Tipo           | Restrições                     |
|------------------|----------------|--------------------------------|
| `PostId`         | UUID            | PRIMARY KEY    |
| `Title`         | VARCHAR(255)   | NOT NULL                       |
| `Slug`           | VARCHAR(255)   | NOT NULL, UNIQUE, LOWERCASE    |
| `Content`       | TEXT           | NOT NULL                       |
| `PublicationDate` | DATETIME       | NOT NULL                       |
| `AuthorId`       | INT            | FOREIGN KEY (`AuthorId`)       |
| `CategoryId`     | INT            | FOREIGN KEY (`CategoryId`)     |
| `Pictures`          | JSON           | NULL                            |

### **4. Tabela: `Comment` (Comentário)**

| Coluna             | Tipo         | Restrições                     |
|--------------------|--------------|--------------------------------|
| `CommentId`        | UUID          | PRIMARY KEY    |
| `Content`         | TEXT         | NOT NULL                       |
| `DateComment`   | DATETIME     | NOT NULL                       |
| `UserId`           | INT          | FOREIGN KEY (`UserId`)         |
| `PostId`           | INT          | FOREIGN KEY (`PostId`)         |
| `ParentCommentId`  | INT          | NULL, FOREIGN KEY (`CommentId`)|

### **5. Tabela: `Tags` (Etiquetas)**

| Coluna           | Tipo           | Restrições                     |
|------------------|----------------|--------------------------------|
| `TagId`          | UUID            | PRIMARY KEY    |
| `Name`           | VARCHAR(255)   | NOT NULL, UNIQUE               |

### **6. Tabela: `PostTags` (Associação N:N entre Posts e Tags)**

| Coluna           | Tipo           | Restrições                     |
|------------------|----------------|--------------------------------|
| `PostId`         | UUID            | FOREIGN KEY (`PostId`)          |
| `TagId`          | UUID            | FOREIGN KEY (`TagId`)           |

### **7. Tabela: `Like` (Curtidas em Posts)**

| Coluna           | Tipo           | Restrições                     |
|------------------|----------------|--------------------------------|
| `LikeId`         | UUID            | PRIMARY KEY    |
| `UserId`         | UUID            | FOREIGN KEY (`UserId`)          |
| `PostId`         | UUID            | FOREIGN KEY (`PostId`)          |
| `DateLike`    | DATETIME       | NOT NULL                       |

---

## **Relacionamentos**

### **1. `Post` e `Category` (1:N)**
- Um **Post** pertence a uma única **Category**.
- Uma **Category** pode ter vários **Posts**.

### **2. `Post` e `User` (1:N)**
- Um **Post** é escrito por um único **User**.
- Um **User** pode escrever vários **Posts**.

### **3. `Post` e `Tags` (N:N)**
- Um **Post** pode ter várias **Tags**, e uma **Tag** pode estar associada a vários **Posts**.

### **4. `Post` e `Comment` (1:N)**
- Um **Post** pode ter vários **Comments**, mas um **Comment** pertence a um único **Post**.

### **5. `User` e `Comment` (1:N)**
- Um **User** pode fazer vários **Comments**, mas um **Comment** pertence a um único **User**.

### **6. `Comment` e `Comment` (1:N)**
- Um **Comment** pode ser uma resposta a outro **Comment** (auto-relacionamento).

### **7. `User` e `Likes` (1:N)**
- Um **User** pode curtir vários **Posts**, mas cada **Like** é único para um **Post** e um **User**.

---

## **Especificações Técnicas**

### **1. Autenticação e Autorização**
- **JWT** será usado para autenticar usuários. O **UserId** será extraído do token JWT para associar ações de posts, comentários e curtidas ao usuário correto.
- Apenas **Admins** e **Escritores** podem criar posts, enquanto **Leitores**, **Escritores**, e **Admins** podem criar comentários.

### **2. Cache**
- **Posts Populares** e **Contagem de Curtidas** serão armazenados em **Cache** (ex: Redis) para reduzir a sobrecarga no banco de dados.
  - O sistema buscará os dados no cache. Caso não existam ou estejam desatualizados, a consulta será feita no banco.
  - O cache será atualizado periodicamente (ex: a cada 10 minutos) para garantir dados mais frescos.
  - Isso evitará sobrecarga em cenários de alta demanda (como 1000 usuários acessando ao mesmo tempo).

---

## **Processo de Envio Formulário**

### **Fluxo do Formulário de Contato**

1. O usuário preenche o formulário de contato no site.
2. Ao submeter, a mensagem é enviada por e-mail para um endereço de e-mail pré-configurado.
3. A mensagem **não será salva no banco de dados**.

### **Regras de Negócio**
- Os campos **nome**, **email**, e **mensagem** são obrigatórios.
- O campo **telefone** é opcional.
- A data e hora do envio são registradas no e-mail.

### **Implementação de E-mail**
- **Tecnologia**: Use um serviço de envio de e-mail como **SendGrid** ou **SMTP** para enviar as mensagens.
- **Validação**: Antes de enviar a mensagem, valide o e-mail e o conteúdo da mensagem.

---

## **Rotas da API**

### **1. Autenticação**

- **POST /api/auth/login**
  - **Descrição**: Realiza o login e retorna um token JWT para o usuário.
  - **Parâmetros**:
    - `email`: string (obrigatório)
    - `senha`: string (obrigatório)
  - **Resposta**:
    - 200 OK: Retorna o token JWT e Refresh token
    - 401 Unauthorized: Credenciais inválidas

- **POST /api/auth/register**
  - **Descrição**: Realiza o cadastro de um novo usuário, incluindo a criação de um usuário com um e-mail único, senha criptografada, e um papel (role). A imagem de perfil será opcional, e o sistema enviará o token JWT para autenticação do usuário.
  - **Parâmetros**
    - **`nome`**: `string` (obrigatório)  
    - **`email`**: `string` (obrigatório, único)  
    - **`senha`**: `string` (obrigatório)  
    - **`role`**: `enum('Admin', 'Writer', 'Reader')` (obrigatório, valor padrão `Reader`)  
    - **`fotoPerfil`**: `file` (opcional)  

  - **Resposta**
    - **201 Created**: Se o usuário for registrado com sucesso, será retornado um código de status **201 Created** com os detalhes do usuário e o token JWT para autenticação.
    - **400 Bad Request**: Caso haja um erro de validação (ex: e-mail já em uso, senha inválida, etc.), será retornado um código **400 Bad Request** com detalhes do erro.

- **POST /api/auth/refresh-token**
  - **Descrição**: Gera um novo token JWT usando o refresh token.
  - **Parâmetros**:
    - `refreshToken`: string (obrigatório)
  - **Resposta**:
    - 200 OK: Retorna o novo token JWT
    - 401 Unauthorized: Refresh token inválido ou expirado

---

### **2. Posts (Artigos)**

- **GET /api/posts**
  - **Descrição**: Retorna uma lista de posts paginada.
  - **Parâmetros**: `page`, `limit`
  - **Resposta**:
    - 200 OK: Lista de posts com paginação
    - 404 Not Found: Nenhum post encontrado

- **GET /api/posts/{id}**
  - **Descrição**: Retorna um post específico pelo `id`.
  - **Parâmetros**: `id`: ID do post
  - **Resposta**:
    - 200 OK: Post encontrado
    - 404 Not Found: Post não encontrado

### **POST /api/posts**
#### Descrição:
Cria um novo post (autorização necessária).

#### Parâmetros:
- `titulo`: string (obrigatório) — O título do post.
- `slug`: string (obrigatório, único) — O identificador único do post, geralmente gerado a partir do título.
- `conteudo`: string (obrigatório) — O conteúdo textual do post.
- `categoriaId`: int (obrigatório) — ID da categoria à qual o post pertence.
- `imagens`: array de arquivos (opcional) — Até 4 imagens podem ser anexadas ao post. As imagens serão armazenadas no servidor em uma pasta de uploads (não no banco de dados). Cada imagem deve ser enviada no formato multipart/form-data.

#### Resposta:
- **201 Created**: Post criado com sucesso, incluindo as imagens.
- **403 Forbidden**: Usuário sem permissão para criar posts.
- **400 Bad Request**: Erro de validação (exemplo: falta de campo obrigatório ou tamanho da imagem maior que o permitido).

---

### **PUT /api/posts/{id}**
#### Descrição:
Atualiza um post existente (apenas o autor ou admin).

#### Parâmetros:
- `id`: ID do post (obrigatório) — O identificador do post a ser atualizado.
- `titulo`: string (opcional) — Novo título para o post.
- `slug`: string (opcional) — Novo slug para o post.
- `conteudo`: string (opcional) — Novo conteúdo do post.
- `categoriaId`: int (opcional) — Nova categoria à qual o post pertence.
- `imagens`: array de arquivos (opcional) — Até 4 imagens podem ser anexadas ao post. As imagens serão armazenadas no servidor em uma pasta de uploads. Caso novas imagens sejam enviadas, elas substituirão as anteriores.

#### Resposta:
- **200 OK**: Post atualizado com sucesso, incluindo as imagens, se fornecidas.
- **403 Forbidden**: Usuário sem permissão para editar o post.
- **404 Not Found**: Post não encontrado pelo ID.

---

### Detalhes adicionais para ambos os métodos:
- **Imagens**: As imagens serão enviadas através do parâmetro `imagens` como parte da requisição no formato `multipart/form-data`. O servidor armazenará essas imagens em uma pasta de uploads, e o banco de dados manterá apenas referências a essas imagens (URLs ou caminhos relativos).
- **Limite de Imagens**: O número máximo de imagens anexadas a um post será de 4.
- **Validação de Imagens**: O tamanho máximo permitido para cada imagem será definido (por exemplo, 5MB por imagem), e o tipo de arquivo permitido será `.jpg`, `.jpeg`, `.png`, ou outros formatos suportados.

---
- **DELETE /api/posts/{id}**
  - **Descrição**: Exclui um post (apenas o autor ou admin).
  - **Parâmetros**: `id`: ID do post
  - **Resposta**:
    - 200 OK: Post deletado
    - 403 Forbidden: Usuário sem permissão
    - 404 Not Found: Post não encontrado

---

### **3. Comentários**

- **GET /api/comments/{postId}**
  - **Descrição**: Retorna a lista de comentários de um post específico.
  - **Parâmetros**: 
    - `postId`: ID do post
  - **Resposta**:
    - 200 OK: Lista de comentários para o post
    - 404 Not Found: Post não encontrado ou sem comentários

- **POST /api/comments**
  - **Descrição**: Cria um novo comentário em um post (autorização necessária).
  - **Parâmetros**:
    - `postId`: ID do post
    - `conteudo`: string (obrigatório)
    - `parentCommentId`: int (opcional, para respostas a outros comentários)
  - **Resposta**:
    - 201 Created: Comentário criado
    - 401 Unauthorized: Usuário não autenticado
    - 400 Bad Request: Erro de validação

- **PUT /api/comments/{id}**
  - **Descrição**: Atualiza um comentário (somente o autor ou admin).
  - **Parâmetros**:
    - `id`: ID do comentário
    - `conteudo`: string (obrigatório)
  - **Resposta**:
    - 200 OK: Comentário atualizado
    - 403 Forbidden: Usuário sem permissão
    - 404 Not Found: Comentário não encontrado

- **DELETE /api/comments/{id}**
  - **Descrição**: Exclui um comentário (somente o autor ou admin).
  - **Parâmetros**: `id`: ID do comentário
  - **Resposta**:
    - 200 OK: Comentário deletado
    - 403 Forbidden: Usuário sem permissão
    - 404 Not Found: Comentário não encontrado

---

### **4. Curtidas**

- **POST /api/likes**
  - **Descrição**: Adiciona uma curtida em um post.
  - **Parâmetros**:
    - `postId`: ID do post
  - **Resposta**:
    - 201 Created: Curtida adicionada
    - 401 Unauthorized: Usuário não autenticado
    - 409 Conflict: Usuário já curtiu o post

- **DELETE /api/likes**
  - **Descrição**: Remove uma curtida de um post.
  - **Parâmetros**:
    - `postId`: ID do post
  - **Resposta**:
    - 200 OK: Curtida removida
    - 401 Unauthorized: Usuário não autenticado
    - 404 Not Found: Curtida não encontrada

---

### **5. Categorias**

- **GET /api/categories**
  - **Descrição**: Retorna uma lista de categorias.
  - **Resposta**:
    - 200 OK: Lista de categorias
    - 404 Not Found: Nenhuma categoria encontrada

- **POST /api/categories**
  - **Descrição**: Cria uma nova categoria (somente admin).
  - **Parâmetros**:
    - `nome`: string (obrigatório, único)
  - **Resposta**:
    - 201 Created: Categoria criada
    - 403 Forbidden: Usuário sem permissão
    - 400 Bad Request: Erro de validação

- **PUT /api/categories/{id}**
  - **Descrição**: Atualiza uma categoria (somente admin).
  - **Parâmetros**:
    - `id`: ID da categoria
    - `nome`: string (obrigatório)
  - **Resposta**:
    - 200 OK: Categoria atualizada
    - 403 Forbidden: Usuário sem permissão
    - 404 Not Found: Categoria não encontrada

- **DELETE /api/categories/{id}**
  - **Descrição**: Exclui uma categoria (somente admin).
  - **Parâmetros**: `id`: ID da categoria
  - **Resposta**:
    - 200 OK: Categoria excluída
    - 403 Forbidden: Usuário sem permissão
    - 404 Not Found: Categoria não encontrada

---

### **6. Tags**

- **GET /api/tags**
  - **Descrição**: Retorna uma lista de tags.
  - **Resposta**:
    - 200 OK: Lista de tags
    - 404 Not Found: Nenhuma tag encontrada

- **POST /api/tags**
  - **Descrição**: Cria uma nova tag (somente admin).
  - **Parâmetros**:
    - `nome`: string (obrigatório, único)
  - **Resposta**:
    - 201 Created: Tag criada
    - 403 Forbidden: Usuário sem permissão
    - 400 Bad Request: Erro de validação

- **DELETE /api/tags/{id}**
  - **Descrição**: Exclui uma tag (somente admin).
  - **Parâmetros**: `id`: ID da tag
  - **Resposta**:
    - 200 OK: Tag excluída
    - 403 Forbidden: Usuário sem permissão
    - 404 Not Found: Tag não encontrada

---

### **7. Envio de Formulário de Contato**

- **POST /api/contact**
  - **Descrição**: Envia uma mensagem de contato via e-mail.
  - **Parâmetros**:
    - `nome`: string (obrigatório)
    - `email`: string (obrigatório)
    - `telefone`: string (opcional)
    - `mensagem`: string (obrigatório)
  - **Resposta**:
    - 200 OK: Mensagem enviada
    - 400 Bad Request: Erro de validação

---

