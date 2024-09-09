# Utilice Vertical Slice Architecture y el patrón de diseño CQRS

![image](https://github.com/user-attachments/assets/f8f42d13-3d90-4778-9787-d028a90e7f61)


# Agregar una migración inicial y actualizar la base de datos
- `Add-Migration InitialCreate`
- `Update-Database`

# Visualizar los mensajes de Kafka
docker exec -it 83a1faf6cf48 kafka-console-consumer.sh --bootstrap-server kafka:9092 --topic permissions-topic --from-beginning

