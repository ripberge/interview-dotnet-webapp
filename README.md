# TixTrack Developer Interview

### Part 1
Analyze the existing code as if you were providing a code review.
Feel free to identify any changes big or small that you would make.
If you have time to rewrite something that is ideal, but if the changes
would be too large you may just provide an explanation and/or comment in the code.

### Part 2
Extend the functionality of this application by providing an API endpoint for cancelling an order.
When an order is cancelled the following must happen:
* The order will have a status of cancelled to differentiate it from an active order
* The order's purchased inventory shall be available for someone else to purchase
* A cancelled order should not be counted in the sales report
* Any other requirements you can think of

