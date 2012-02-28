CREATE TABLE account
(
  id integer NOT NULL,
  "login" character varying(30) NOT NULL,
  "password" character varying(30) NOT NULL,
  email character varying(100) NOT NULL,
  CONSTRAINT "PK_ACCOUNT" PRIMARY KEY (id),
  CONSTRAINT "UK_ACCOUNT_LOGIN" UNIQUE (login)
);

CREATE TABLE "character"
(
  id integer NOT NULL,
  "name" character varying(30) NOT NULL,
  account_id integer NOT NULL,
  CONSTRAINT "PK_CHARACTER" PRIMARY KEY (id),
  CONSTRAINT "FK_CHARACTER_ACCOUNT" FOREIGN KEY (account_id)
      REFERENCES account (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE,
  CONSTRAINT "UK_CHARACTER_NAME" UNIQUE (name)
);

