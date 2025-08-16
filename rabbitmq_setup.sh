#!/bin/bash
set -e

# Wait until RabbitMQ is ready
until rabbitmqctl status; do
  echo "Waiting for RabbitMQ..."
  sleep 2
done

# Declare exchanges
rabbitmqadmin declare exchange name=account.events type=topic durable=true

# Declare queues
rabbitmqadmin declare queue name=account.crm durable=true
rabbitmqadmin declare queue name=account.notifications durable=true
rabbitmqadmin declare queue name=account.antifraud durable=true
rabbitmqadmin declare queue name=account.audit durable=true

# Declare bindings
rabbitmqadmin declare binding source=account.events destination=account.crm destination_type=queue routing_key="account.*"
rabbitmqadmin declare binding source=account.events destination=account.notifications destination_type=queue routing_key="money.*"
rabbitmqadmin declare binding source=account.events destination=account.antifraud destination_type=queue routing_key="client.*"
rabbitmqadmin declare binding source=account.events destination=account.audit destination_type=queue routing_key="#"
