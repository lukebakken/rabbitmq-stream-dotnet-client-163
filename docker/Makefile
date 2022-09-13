.PHONY: clean down fresh perms setup up

clean: down perms
	rm -rf $(CURDIR)/mnesia/*/rabbit*

down:
	docker compose down

perms:
	sudo chown -R "$(USER):$(USER)" .

setup:
	sudo chmod 0600 erlang.cookie
	sudo chown -R 999 mnesia

up:
	docker compose up --detach

fresh: down clean up
