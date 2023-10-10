# HotelCompare
An Azure OpenAI powered Hotel Review Comparison tool.

This project is the backend API. The client application is stored in a different repository.

# Goal
To automate the hotel comparison process based on their reviews on booking.com (and/or the likes)

# Technologies 
- .Net 7
- Azure Cognitive Search
- Semantic Kernel
- Azure OpenAI

# Concepts Used
- Some of the main GenAI capabilities to summarise texts, infer topics, analyse sentiments.
- Prompt Engineering
- Semantic Kernel
- Semantic Memory

# Input 
Phase 1:
- Up to 5 hotels to compare
- User preferences in a text format (i.e. I want a hotel with good location, close to city center, cheap, good value, good food, etc)

# Output 
Phase 1: Each hotel reviews will be analysed and the app will come up with the followings:
- popular sentiment from the whole reviews
- what feature of the hotel that the guests like the most
- what feature of the hotel that the guests dislike the most
- summary of the whole reviews

The application then will analyse results against the user preferences provided and pick the top 2 recommended hotels along with the reason why.

# Project Structure
HotelCompareWebAPI
├── Controllers             
├── Data            # Dummy sample of hotel reviews
├── Extensions      # Objects used during the application startup, containing processes to load configuration, initiate semantic kernel, and register skills
├── Model                   
├── Services                
├── Skills          # This folder is where the skills are stored, containing folders with a set of skill prompt and config
├── Utilities        
├── Program.cs
└── appsettings.json

# How to Run
1. Enter the details of your Azure OpenAI (Key, Endpoint, and Deployment) as well as the configurations for Text Embedding and Azure Cognitive Service in appsettings.json.
2. Run the Web API
3. Execute the API by sending a REST GET request, i.e. 	`code` http://localhost:7133/compare?hotelName=<hotel names separated by comma>&userPreferences=<user preferences as text> 	`code`
