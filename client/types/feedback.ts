export interface CreateFeedbackDto {
  userId: string;
  orderId: string;
  rating: number;
  content: string;
}
