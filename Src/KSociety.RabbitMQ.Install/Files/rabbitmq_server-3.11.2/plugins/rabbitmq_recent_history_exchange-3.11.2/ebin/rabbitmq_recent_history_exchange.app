{application, 'rabbitmq_recent_history_exchange', [
	{description, "RabbitMQ Recent History Exchange"},
	{vsn, "3.11.2"},
	{id, "v3.11.2"},
	{modules, ['rabbit_exchange_type_recent_history']},
	{registered, []},
	{applications, [kernel,stdlib,rabbit_common,rabbit]},
	{env, []},
		{broker_version_requirements, []}
]}.