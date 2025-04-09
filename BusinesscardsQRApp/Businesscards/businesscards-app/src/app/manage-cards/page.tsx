"use client";
import React, {useEffect, useState} from 'react';
import axios from 'axios';
import { Eye } from "lucide-react";
import Image from "next/image";

interface BusinessCard {
    id: string;
    fullName: string;
    title: string;
    email: string;
    phone: string;
    companyName: string;
    website: string;
    address: string;
  }

  interface PaginationMetadata {
    CurrentPage: number;
    TotalPages: number;
    PageSize: number;
    TotalCount: number;
    HasPrevious: boolean;
    HasNext: boolean;
  }

export default function ManageCard() {
    debugger;

    const [cards, setCards] = useState<BusinessCard[]>([]);
    const [pagination, setPagination] = useState<PaginationMetadata | null>(null);
    const [pageNumber, setPageNumber] = useState(1);
    const pageSize = 10;

    const [selectedCard, setSelectedCard] = useState<BusinessCard | null>(null);
    const [showModal, setShowModal] = useState(false);

    const fetchCards = async (page: number) => {
        try {
          const response = await axios.get(
            `https://localhost:7267/api/BusinessCards/bussinessusers?PageNumber=${page}&PageSize=${pageSize}`
          );
          console.log(response);
          console.log(response.data);
          console.log(response.headers);
          console.log(response.status);
          const paginationHeader = 
            response.headers['x-pagination'] || 
            response.headers['X-Pagination'] ||
            response.headers['X-pagination'];

        // Log all headers for debugging
        const headerObj = Object.fromEntries(
            Object.entries(response.headers)
        );
        console.log('Headers as object:', headerObj);

        if (paginationHeader) {
            const parsed = JSON.parse(paginationHeader);
            console.log("Parsed Pagination Header:", parsed);
            setPagination(parsed);
          }
          else{
            console.log("pagination header not available");
          }
    
          setCards(response.data);
        } catch (error) {
          console.error("Error fetching business cards", error);
        }
      };
    
      useEffect(() => {
        fetchCards(pageNumber);
      }, [pageNumber]);
      
      const handlePrev = () => {
        if (pagination?.HasPrevious) {
          setPageNumber(prev => prev - 1);
        }
      };
    
      const handleNext = () => {
        if (pagination?.HasNext) {
          setPageNumber(prev => prev + 1);
        }
      };

      const handleView = (card: BusinessCard) => {
        setSelectedCard(card);
        setShowModal(true);
      };
      
      const handleCloseModal = () => {
        setShowModal(false);
        setSelectedCard(null);
      };

      return (
        <div className="pl-4 py-6">
          <h1 className="text-2xl font-bold mb-4">Manage Business Cards</h1>
      
          <div className="w-full overflow-x-auto"> {/* Changed to w-full */}
            <table className="min-w-max bg-white shadow-md rounded-lg"> {/* Changed to min-w-max */}
              <thead className="bg-gradient-to-r from-blue-500 to-blue-700 text-white">
                <tr>
                  <th className="text-left px-4 py-3 text-sm font-medium uppercase tracking-wider whitespace-nowrap">Full Name</th>
                  <th className="text-left px-4 py-3 text-sm font-medium uppercase tracking-wider whitespace-nowrap">Title</th>
                  <th className="text-left px-4 py-3 text-sm font-medium uppercase tracking-wider whitespace-nowrap">Email</th>
                  <th className="text-left px-4 py-3 text-sm font-medium uppercase tracking-wider whitespace-nowrap">Phone</th>
                  <th className="text-left px-4 py-3 text-sm font-medium uppercase tracking-wider whitespace-nowrap">Company</th>
                  <th className="text-left px-4 py-3 text-sm font-medium uppercase tracking-wider whitespace-nowrap">Website</th>
                  <th className="text-left px-4 py-3 text-sm font-medium uppercase tracking-wider whitespace-nowrap">Address</th>
                  <th className="text-left px-4 py-3 text-sm font-medium uppercase tracking-wider whitespace-nowrap">Actions</th>
                </tr>
              </thead>
              <tbody>
                {cards.map((card, index) => (
                  <tr
                    key={card.id}
                    className={index % 2 === 0 ? "bg-gray-50 hover:bg-gray-100" : "bg-white hover:bg-gray-100"}>
                    <td className="px-4 py-3 text-sm text-gray-700 whitespace-nowrap">{card.fullName}</td>
                    <td className="px-4 py-3 text-sm text-gray-700 whitespace-nowrap">{card.title}</td>
                    <td className="px-4 py-3 text-sm text-blue-600 whitespace-nowrap">{card.email}</td>
                    <td className="px-4 py-3 text-sm text-gray-700 whitespace-nowrap">{card.phone}</td>
                    <td className="px-4 py-3 text-sm text-gray-700 whitespace-nowrap">{card.companyName}</td>
                    <td className="px-4 py-3 text-sm text-blue-600 underline whitespace-nowrap">
                      <a href={card.website} target="_blank" rel="noopener noreferrer">{card.website}</a>
                    </td>
                    <td className="px-4 py-3 text-sm text-gray-700 whitespace-nowrap">{card.address}</td>
                    <td className="px-4 py-3 whitespace-nowrap">
                      <button
                        onClick={() => handleView(card)}
                        className="text-blue-600 hover:text-blue-800"
                        title="View Card">
                        <Eye className="w-5 h-5" />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
      
          <div className="mt-4 flex items-center gap-4">
            <button
              onClick={handlePrev}
              disabled={!pagination?.HasPrevious}
              className="bg-gray-300 hover:bg-gray-400 text-black font-semibold px-4 py-2 rounded disabled:opacity-50"
            >
              Previous
            </button>
      
            <p>
              Page {pagination?.CurrentPage} of {pagination?.TotalPages}
            </p>
      
            <button
              onClick={handleNext}
              disabled={!pagination?.HasNext}
              className="bg-gray-300 hover:bg-gray-400 text-black font-semibold px-4 py-2 rounded disabled:opacity-50"
            >
              Next
            </button>
          </div>
          
        </div>
      );
      
      
    }      