# RAG .NET Web API

This project is a **Retrieval-Augmented Generation (RAG) system** built using **.NET Web API**. It integrates **Qdrant** as a **vector database** for storing and retrieving embeddings with met---ata. The API allows users to store and query documents efficiently, enabling intelligent search and retrieval for AI-powered applications.

---

## 🚀 Features

- Store embeddings with metadata in **Qdrant**.
- Perform **semantic search** over stored documents.
- Easy integration with **LLMs** for RAG-based applications. (We started with Mistral API)
- **REST API** endpoints for inserting and querying data.
- Built with **.NET** for high performance.

---

## 📌 Prerequisites

Ensure you have the following installed:

- **Docker** **Desktop** (For running Qdrant locally) - [Install](https://docs.docker.com/get-docker/)

- **Qdrant**: Run the following command to start a Qdrant instance:

  ```sh
  docker run -p 6333:6333 -p 6334:6334 qdrant/qdrant
  ```

  This command will create and start a **Qdrant container**. You can stop it using Docker Desktop or by closing the terminal. To restart it, simply use Docker Desktop.

- **Mistral API Key**: You need to obtain an API key from [Mistral](https://mistral.ai/) and set it in the `.env` file under `MISTRAL_API_KEY`.

- **Embedding Model**: Download the embedding model from [Hugging Face](https://huggingface.co/Qdrant/paraphrase-multilingual-MiniLM-L12-v2-onnx-Q/tree/main). You need to download the following files:

  - `model.onnx`
  - `tokenizer.json`

  Then, create a new folder in the root directory named `ModelAi` and place both files inside it.

---

## 🔧 Installation

1. **Clone the repository:**
   ```sh
   git clone https://github.com/nvotec/nexia-api.git
   cd rag-dotnet-webapi
   ```
2. **Install dependencies:**
   ```sh
   dotnet restore
   ```
3. **Set up environment variables:**
   - Create a `.env` file in the root directory and add the following environment variable:
     ```env
     MISTRAL_API_KEY=PASTE_YOUR_API_HERE
     ```

---

## ▶️ Running the Project

### **1️⃣ Start Qdrant**

Ensure Qdrant is running on `http://localhost:6333`. You can also access the **Qdrant UI Dashboard** at `http://localhost:6333/dashboard` to monitor and manage your collections.

### **2️⃣ Run the API**

```sh
 dotnet run
```

You can use **Swagger** at `http://localhost:{your_port}/swagger` to test the endpoints.

---

## 📡 API Endpoints

### **📂 Document Management**

#### **1️⃣ Save a Document**

- **POST** `/api/Document/save-document`
- **Description:** Saves a document with embeddings and metadata.
- **Request Body:**
  ```json
  {
      "category": "Your Category",
      "collectionName": "Your Collection Name",
      "filepath": "Your/File/Path"
  }
  ```

#### **2️⃣ List Document Collections**

- **GET** `/api/Document/list-collections`
- **Description:** Retrieves the list of stored document collections.

#### **3️⃣ Delete Document by Filename**

- **DELETE** `/api/Document/delete-by-filename`
- **Description:** Deletes a document from a collection based on the filename.
- **Request Body:**
  ```json
  {
    "collectionName": "Your Collection Name",
    "filename": "Your File Name"
  }
  ```

---

### **🤖 Mistral AI Integration**

#### **4️⃣ Chat with LLM**

- **POST** `/api/mistral/chatWithLLM`
- **Description:** Sends a message to the Mistral API and retrieves a response.
- **Request Body:**
  ```json
  {
    "message": "Your input text here"
  }
  ```

#### **5️⃣ Generate with RAG**

- **POST** `/api/mistral/generateWithRag`
- **Description:** Uses RAG (Retrieval-Augmented Generation) to generate a response based on stored documents.
- **Request Body:**
  ```json
  {
    "userInput": "Your query text here",
    "collectionName": "your-collection"
  }
  ```

---

## 🧪 RAG Test Example

To ensure the RAG pipeline is functioning correctly, we provide test data:

- **Test Document**: [Solar Energy & Solar Panels](https://drive.google.com/file/d/1SUs9SHkCevpfuVoPyy7E8YStfgBicVGe/view?usp=sharing)
- **Test Questions & Expected Answers**: [Question-Answer Set](https://drive.google.com/file/d/1fvol2vhCl42J71LxRbdN7pe2mBjXmkZJ/view?usp=sharing)

The first text file contains the test data (Solar Energy & Solar Panels), and the second text file contains sample questions that users can ask the LLM to verify if the RAG pipeline is functioning correctly.

