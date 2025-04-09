"use client"
import React, { useState } from "react";
import axios from "axios";

interface BusinessCard{
    fullName: string;
    title: string;
    email: string;
    phone: string;
    companyName: string;
    website: string;
    address: string;
    linkedInUrl: string;
    githubUrl: string;
    personalBlobUrl:string;
    profileImageUrl:string;
}

export default function BusinessCardForm() {
    const [formData, setFormData] = useState<BusinessCard>({
        fullName: "",
        title:"",
        email: "",
        phone: "",
        companyName: "",
        website: "",
        address: "",
        linkedInUrl: "",
        githubUrl: "",
        personalBlobUrl:"",
        profileImageUrl:"",
    });

    const [qrCodeUrl, setQrCodeUrl] = useState<string | null>(null);
    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
      };
    
      const checkApiHealth = async () => {
        try {
            await axios.get('https://localhost:7267/health');
            return true;
        } catch (error) {
            console.error('API is not accessible:', error);
            return false;
        }
    };

      const handleSubmit = async (e: React.FormEvent) => {
        debugger;
        e.preventDefault();

         // Check if API is accessible
        const isApiHealthy = await checkApiHealth();
        if (!isApiHealthy) {
            console.error('API is not accessible');
           // return;
        }

        // Basic Validation
        if (!formData.fullName || !formData.email || !formData.phone || !formData.companyName) {
            console.error("Please fill in all required fields.");
            return;
        }

        try {
          
            const response = await axios.post(
            "https://localhost:7267/api/QRCode/generatewithlogo",
            formData,
            { responseType: "blob",
                headers:{
                    'Content-Type':'application/json',
                    'Accept':'application/json'
                }
             }
          );
          const url = URL.createObjectURL(response.data);
          setQrCodeUrl(url);
        } catch (error: any) {
            if (error.response) {
                // Handle blob error response
                const blob = error.response.data;
                const reader = new FileReader();
                
                reader.onload = () => {
                    try {
                        const errorData = JSON.parse(reader.result as string);
                        console.error('Detailed error:', errorData);
                    } catch (e) {
                        console.error('Raw error response:', reader.result);
                    }
                };
                
                reader.onerror = () => {
                    console.error('Failed to read error response');
                };
                
                reader.readAsText(blob);
    
                console.error('Status:', error.response.status);
                console.error('Headers:', error.response.headers);
            } else if (error.request) {
                console.error('No response received:', error.request);
            } else {
                console.error('Error:', error.message);
            }
        }
      };
    
      return (
        <div>
          <h1 className="text-2xl font-bold mb-4">Create Digital Business Card</h1>
          <form onSubmit={handleSubmit} className="space-y-3">
            {Object.keys(formData).map(key => (
              <input
                key={key}
                type="text"
                name={key}
                placeholder={key}
                value={(formData as any)[key]}
                onChange={handleChange}
                className="w-full p-2 border rounded"
              />
            ))}
            <button type="submit" className="bg-blue-500 text-white px-4 py-2 rounded">
              Generate QR Code
            </button>
          </form>
    
          {qrCodeUrl && (
            <div className="mt-6 border p-4 rounded shadow-md">
              <h2 className="text-xl font-semibold mb-2">{formData.fullName}</h2>
              <p>{formData.title} @ {formData.companyName}</p>
              <p>Email: {formData.email}</p>
              <p>Phone: {formData.phone}</p>
              <p>Website: {formData.website}</p>
              <p>Address: {formData.address}</p>
              {formData.linkedInUrl && <p>LinkedIn: {formData.linkedInUrl}</p>}
              {formData.githubUrl && <p>GitHub: {formData.githubUrl}</p>}
              {formData.personalBlobUrl && <p>Blog: {formData.personalBlobUrl}</p>}
              {formData.profileImageUrl && <p>Profile Image: {formData.profileImageUrl}</p>}
              <div className="mt-4">
                <img src={qrCodeUrl} alt="QR Code" className="w-40 h-40" />
              </div>
            </div>
          )}
        </div>
    );
}