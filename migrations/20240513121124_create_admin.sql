-- +goose Up
-- +goose StatementBegin
delete from users where guid = '00000000-0000-0000-0000-000000000001';

insert into users (guid, login, password, name, gender, is_admin, created_on, created_by, modified_on, modified_by, revoked_on, revoked_by)
values ('00000000-0000-0000-0000-000000000001', 'admin', 'admin', 'admin', 0, true, now(), 'admin', now(), 'admin', null, null);
-- +goose StatementEnd

-- +goose Down
-- +goose StatementBegin
delete from users where guid = '00000000-0000-0000-0000-000000000001';
-- +goose StatementEnd