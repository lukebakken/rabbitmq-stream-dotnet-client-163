.PHONY: clean down fresh setup up

clean: down
	sudo chown -R "$(USER):$(USER)" .
	rm -rf $(CURDIR)/mnesia/*/rabbit*

down:
	docker compose down

setup:
	sudo chown -R 999 mnesia

up:
	docker compose up --detach

fresh: down clean up
