# Proyecto de Scraping

Este proyecto de scraping es una colección de clases diseñadas para extraer datos de diversas fuentes en línea, como páginas web y archivos PDF, y almacenarlos en una base de datos. Cada clase se enfoca en un sitio web específico o una fuente de datos particular y utiliza técnicas de scraping para recopilar información relevante.

## Descripción General

El proyecto consta de varias clases, cada una destinada a extraer datos de una fuente específica y procesarlos de manera adecuada para su almacenamiento en la base de datos. Estas clases están diseñadas para ser modulares y fáciles de mantener, permitiendo la adición de nuevas funcionalidades o la modificación de las existentes según sea necesario.

### Clases de Scraping

1. **Request.cs**: Esta clase se encarga de realizar solicitudes HTTP a una página web específica que lista los geólogos registrados en Costa Rica. Luego procesa el HTML obtenido para extraer los nombres y otros datos relevantes de los geólogos y los guarda en la base de datos.

2. **Request2.cs**: Similar a Request.cs, esta clase está adaptada para extraer datos de otra página web que lista miembros de una asociación profesional. Utiliza las mismas técnicas de scraping para recopilar la información requerida y almacenarla en la base de datos.

3. **ScrapHuli.cs**: Esta clase se conecta a una API que proporciona información sobre médicos y cirujanos en Costa Rica. Utiliza solicitudes HTTP para obtener los datos de la API, los procesa y los guarda en la base de datos para su posterior consulta.

4. **ScrapColyPro.cs**: Diseñada para trabajar con archivos PDF, esta clase descarga un archivo PDF de una página web que contiene una lista de colegiados. Luego, utiliza la biblioteca iTextSharp para extraer el texto del PDF, procesa los datos y los almacena en la base de datos.

## Implementaciones Específicas

### Bulk Insert y Multithreading

- Se ha implementado el uso de Bulk Insert para mejorar el rendimiento al insertar grandes cantidades de datos en la base de datos. Esto se logra mediante la inserción de lotes de datos en lugar de hacer una inserción individual para cada registro, lo que reduce la sobrecarga y mejora la velocidad de procesamiento.
  
- Se utilizan estructuras de datos concurrentes como ConcurrentBag para recopilar datos de forma segura en entornos multihilo. Esto permite que múltiples hilos accedan y modifiquen la colección al mismo tiempo sin conflictos de concurrencia.

- Se aprovecha la clase Parallel.ForEachAsync para procesar datos en paralelo y mejorar la velocidad de extracción. Esta clase permite ejecutar iteraciones de bucle en paralelo, lo que puede aumentar significativamente el rendimiento al realizar tareas intensivas en CPU de manera concurrente.

### Búsqueda y Creación de Colegios

- En la clase ScrapColyPro.cs, se busca un colegio específico en la base de datos (por ejemplo, el "Colegio de Licenciados y Profesores"). Si el colegio no está presente, se crea automáticamente para asociar los colegiados extraídos. Esto garantiza que todos los colegiados estén correctamente vinculados a su institución correspondiente en la base de datos.

## Dependencias

- **HtmlAgilityPack**: 1.11.61
- **iTextSharp**: 5.5.13.3
- **RestSharp**: 110.2.0
- **EFCore.BulkExtensions**: 8.0.3
- **Newtonsoft.Json**: 13.0.3

## Entorno de Desarrollo

- **Visual Studio**: 2022 (v17.10.0)
- **.NET Core SDK**: 8.0
- **C#**: 9.0

## Contribución

¡Tu contribución es bienvenida! Si deseas colaborar en este proyecto, sigue estos pasos:

1. Haz un fork del repositorio.
2. Crea una nueva rama para tus cambios.
3. Realiza tus modificaciones y asegúrate de seguir las convenciones de codificación y estilo existentes.
4. Envía un pull request describiendo tus cambios y explicando su propósito.
5. Revisa cualquier comentario o retroalimentación proporcionada y realiza ajustes según sea necesario.

## Licencia

Este proyecto está licenciado bajo la Licencia MIT. Consulta el archivo LICENSE.md para obtener más detalles.
