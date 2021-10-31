{application, 'rabbitmq_federation_management', [
	{description, "RabbitMQ Federation Management"},
	{vsn, "3.9.8"},
	{id, "v3.9.8"},
	{modules, ['rabbit_federation_mgmt']},
	{registered, []},
	{applications, [kernel,stdlib,rabbit_common,rabbit,rabbitmq_management,rabbitmq_federation]},
	{env, []},
		{broker_version_requirements, []}
]}.