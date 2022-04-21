{application, 'rabbitmq_shovel_management', [
	{description, "Management extension for the Shovel plugin"},
	{vsn, "3.9.15"},
	{id, "v3.9.15"},
	{modules, ['rabbit_shovel_mgmt']},
	{registered, []},
	{applications, [kernel,stdlib,rabbit_common,rabbit,rabbitmq_management,rabbitmq_shovel]},
	{env, []},
		{broker_version_requirements, []}
]}.