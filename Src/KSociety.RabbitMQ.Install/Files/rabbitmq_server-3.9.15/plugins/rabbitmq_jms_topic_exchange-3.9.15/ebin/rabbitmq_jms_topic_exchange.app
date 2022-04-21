{application, 'rabbitmq_jms_topic_exchange', [
	{description, "RabbitMQ JMS topic selector exchange plugin"},
	{vsn, "3.9.15"},
	{id, "v3.9.15"},
	{modules, ['rabbit_jms_topic_exchange','sjx_evaluator']},
	{registered, []},
	{applications, [kernel,stdlib,rabbit_common,rabbit]},
	{env, []}
]}.