-- +goose Up
-- +goose StatementBegin
create table if not exists users (
   guid uuid,
   login text,
   password text,
   name text,
   gender int,
   birthday timestamp,
   is_admin boolean,
   created_on timestamp,
   created_by text,
   modified_on timestamp,
   modified_by text,
   revoked_on timestamp,
   revoked_by text
)
-- +goose StatementEnd

-- +goose Down
-- +goose StatementBegin
drop table if exists users;
-- +goose StatementEnd