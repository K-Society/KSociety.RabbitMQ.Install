{application, 'rabbitmq_stream_common', [
	{description, "RabbitMQ Stream Common"},
	{vsn, "3.11.2"},
	{id, "v3.11.2"},
	{modules, ['rabbit_stream_core']},
	{registered, []},
	{applications, [kernel,stdlib]},
	{env, [
]}
]}.