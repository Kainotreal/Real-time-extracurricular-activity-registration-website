from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity
import numpy as np

app = FastAPI()

class Activity(BaseModel):
    id: int
    text: str

class RecommendationRequest(BaseModel):
    target_id: int
    activities: List[Activity]
    top_k: int = 4

class RecommendationResponse(BaseModel):
    recommended_ids: List[int]

@app.post("/recommend", response_model=RecommendationResponse)
def recommend(req: RecommendationRequest):
    if not req.activities:
        return RecommendationResponse(recommended_ids=[])
        
    # Find active target
    target_idx = -1
    for i, act in enumerate(req.activities):
        if act.id == req.target_id:
            target_idx = i
            break
            
    if target_idx == -1:
        # Target activity not in the provided list
        return RecommendationResponse(recommended_ids=[])

    if len(req.activities) <= 1:
        return RecommendationResponse(recommended_ids=[])

    # Extract texts
    texts = [act.text for act in req.activities]
    
    # Compute TF-IDF
    vectorizer = TfidfVectorizer(stop_words=None) # We can add Vietnamese stopwords if needed
    tfidf_matrix = vectorizer.fit_transform(texts)
    
    # Compute cosine similarity for the target activity
    cosine_sim = cosine_similarity(tfidf_matrix[target_idx:target_idx+1], tfidf_matrix).flatten()
    
    # Get top K indices (excluding the target itself)
    # argsort sorts in ascending order, so we take from the end
    sorted_indices = cosine_sim.argsort()[::-1]
    
    top_k_indices = []
    for idx in sorted_indices:
        if idx != target_idx:
            top_k_indices.append(idx)
            if len(top_k_indices) == req.top_k:
                break
                
    recommended_ids = [req.activities[idx].id for idx in top_k_indices]
    
    return RecommendationResponse(recommended_ids=recommended_ids)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8000)
