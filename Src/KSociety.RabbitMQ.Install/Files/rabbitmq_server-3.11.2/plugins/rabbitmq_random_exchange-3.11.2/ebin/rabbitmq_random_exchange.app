{application, 'rabbitmq_random_exchange', [
	{description, "RabbitMQ Random Exchange"},
	{vsn, "3.11.2"},
	{id, "v3.11.2"},
	{modules, ['rabbit_exchange_type_random']},
	{registered, []},
	{applications, [kernel,stdlib,rabbit_common,rabbit]},
	{env, []}
]}.